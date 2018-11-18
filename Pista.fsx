open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

#load "Strada.fsx"
#load "Block.fsx"
#load "Mod.fsx"
#load "Car.fsx"
#load "Risultati.fsx"

type Pista() =
    let mutable completed = false
    let circuito = new ResizeArray<Strada.Strada>()
    let toolbar = new ResizeArray<Block.Block>()
    let change = new ResizeArray<Mod.Mod>()
    let mutable laps = 0
    let mutable cars = new ResizeArray<Car.Car>()
    let winners = new ResizeArray<Car.Car>()
    let mutable maxx, maxy, minx, miny = 0, 0, 0, 0
    let mutable handle = RectangleF()
    let mutable index  = 0
    let mutable macchine = 0
    let mutable start = false
    let res = new Risultati.Risultati()
    
    ///restituisce il vettore delle macchine partecipanti
    member this.GetCars
        with get() = cars

    member this.Add
        with set( v ) = circuito.Add( v )

    member this.RemoveLast() =
        if circuito.Count > 0 then
            let last = circuito.Item( circuito.Count - 1 )
            circuito.RemoveAt( circuito.Count - 1 )
            last
        else
            null

    /// decreta la fine della gara
    member this.EndRace() =
        winners.Count = macchine

    /// restituisce il numero di giri da compiere
    member this.Laps
        with get() = laps
        and set( v ) = laps <- v

    /// setta/restituisce il numero di macchine participanti
    member this.Macchine
        with get() = macchine
        and set( v ) = macchine <- v

    member this.Start( mIndex, startX, startY, laps ) = 
        winners.Clear()

        for i in 1 .. macchine do
            if i = 1 then
                cars.Add( Car.Car( index = mIndex, Position = PointF( single (startX), single (startY) ), Number = i - 1, Tratto = 0, Laps = laps, Speed = 1.f, Basic = 1.f ) )
            else if i = 2 then
                cars.Add( Car.Car( index = mIndex, Position = PointF( single (startX), single (startY) ), Number = i - 1, Tratto = 0, Laps = laps, Speed = 2.f, Basic = 2.f ) )
            else if i = 3 then
                cars.Add( Car.Car( index = mIndex, Position = PointF( single (startX), single (startY) ), Number = i - 1, Tratto = 0, Laps = laps, Speed = 4.f, Basic = 4.f ) )
            else if i = 4 then
                cars.Add( Car.Car( index = mIndex, Position = PointF( single (startX), single (startY) ), Number = i - 1, Tratto = 0, Laps = laps, Speed = 0.5f, Basic = 0.5f ) )

        start <- true

    member this.maxX
        with get() = maxx
        and set( v ) = maxx <- v

    member this.minX
        with get() = minx
        and set( v ) = minx <- v

    member this.minY
        with get() = miny
        and set( v ) = miny <- v

    member this.maxY
        with get() = maxy
        and set( v ) = maxy <- v

    ///setta e restituisce il rettangolo per lo spostamento
    member this.Handle
        with set( v ) = handle <- v
        and get() = handle

    member this.Contains( e:PointF ) =
        handle.Contains( e.X, e.Y )

    member this.Result
        with get() = res

    /// controlla se e' stato premuto un tasto dei settaggi
    member this.CheckPressedSettings( p:PointF ) =
        //printfn "SIZE: %d, %d" toolbar.Count change.Count
        let mutable pressed = false
        let mutable found = false
        for blocco in toolbar do
            if blocco.Contains( p ) then
                found <- true
                for c in change do
                    if c.Contains( p ) then
                        pressed <- true
                        if blocco.Index = 0 then
                            if c.Index = 0 then
                                laps <- laps + 1
                            else
                                laps <- max 0 (laps - 1)
                        else
                            if c.Index = 0 then
                                macchine <- min 4 (macchine + 1)
                            else
                                macchine <- max 0 (macchine - 1)
        
        pressed
            
    /// assegna/restituisce il numero del circuito
    member this.Index 
        with get() = index
        and set( v ) = index <- v        

    /// restituisce la lista d'arrivo
    member this.getArrive
        with get() = winners

    /// restituisce il numero di strade nel vettore circuito
    member this.Count
        with get() = circuito.Count

    /// restituisce il vettore circuito
    member this.GetCircuit
        with get() = circuito

    /// restituisce i blocchi di modifica gara
    member this.GetBlocks
        with get() = toolbar

    /// aggiunge i riquadri relativi a giri e macchine
    member this.AddBlock( p:PointF, index:int ) =
        toolbar.Add( Block.Block( Position = p, Index = index ) )

    /// aggiunge i riquadri per settare i giri e le macchine
    member this.AddMod( p:PointF, index:int ) =
        change.Add( Mod.Mod( Position = p, Index = index ) )

    /// restituisce le sezioni per la modifica delle impostazioni
    member this.GetMod
        with get() = change

    /// determina se il circuito e' completato
    member this.Completed
        with get() = completed
        and set( v ) = completed <- v

    /// ordina le macchine in base al tratto di strada
    member private this.SortCars() =
        let orderedCars = new ResizeArray<Car.Car>()
        for i in 1 .. macchine do
            let mutable min = 0
            let mutable index, size = 1, cars.Count
            while index < size do
                if cars.Item( index ).Tratto < cars.Item( min ).Tratto then
                    min <- index
                index <- index + 1
            orderedCars.Add( cars.Item( min ) )
            cars.RemoveAt( min ) |> ignore

        cars <- orderedCars

    /// stampa i tracciati da un punto di inzio a uno di fine
    member private this.PrintTracks( g:Graphics, from:int, To:int ) =
        for i in from .. To do
            circuito.Item( i ).paint( g )

    /// disegna la pista
    member this.Paint( g:Graphics, f:Font ) =
        if start then
            let mutable toSort = false

            for m in cars do
                if not m.Finished then
                    m.updateCar( m, circuito.Item( m.Tratto ), circuito.Count, circuito )
                    toSort <- true

            if toSort then
                this.SortCars()

            // itero sulle macchine ordinate per posizione
            let mutable index = 0
            for m in cars do
                if not m.Finished then
                    this.PrintTracks( g, index, m.Tratto )
                    index <- m.Tratto + 1
                    m.Paint( g )
                else
                    if not( winners.Contains( m ) ) then
                        winners.Add( m )

                    if winners.Count = macchine then
                        start <- false
                        res.Visible <- true

            this.PrintTracks( g, index, circuito.Count - 1 )
        else
            for r in circuito do
                r.paint( g )

        if completed then
            g.DrawRectangle( Pens.Red, handle.X, handle.Y, handle.Width, handle.Height )
            let lap = sprintf "LAPS"
            g.DrawString( lap, f, Brushes.Black, RectangleF( single maxx - 50.f + 15.f, single maxy - 79.f - 5.f, 40.f, 20.f ) )
            for a in change do
                a.Paint( g )
            let lap = sprintf "CARS"
            g.DrawString( lap, f, Brushes.Black, RectangleF( single maxx - 50.f + 15.f, single maxy - 79.f + 40.f, 40.f, 20.f ) )
            for t in toolbar do
                t.Paint( g )

            let s = sprintf "%d " laps
            g.DrawString( s, f, Brushes.Black, RectangleF( single maxx - 50.f + 25.f, single maxy - 79.f + 15.f, 40.f, 20.f ) )

            let s = sprintf "%d " macchine
            g.DrawString( s, f, Brushes.Black, RectangleF( single maxx - 50.f + 25.f, single maxy - 79.f + 60.f, 40.f, 20.f ) )

        if res.Visible then
            res.Paint( g, winners, index, PointF( single minx, single miny ), SizeF( single( maxx - minx ), single( maxy - miny ) ) )
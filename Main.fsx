open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

#load "ToolbarButton.fsx"
#load "ManipolationButton.fsx"
#load "ScrollButton.fsx"
#load "Strada.fsx"
#load "Pista.fsx"

let f = new Form( Text = "Mid2016", TopMost = true, Width = 900, Height = 600  )

type Main() as this =
    inherit UserControl()
    
    // lista di strade create
    let pista = new ResizeArray<Strada.Strada>()
    // raccolta di tutte le pista create
    let mondiale = new ResizeArray<Pista.Pista>()
    // determina il numero di piste completate
    let mutable tracks = -1
    // determina il numero di strade in quella pista
    let mutable count = 0
    // lunghezza e altezza dei bottoni di creazione
    let btnw, btnh = 40, 40
    // timer per scrollare tenendo premuto il mouse sopra una freccia
    let timerscroll = new Timer( Interval = 100 )
    // determina se il mouse e' stato cliccato e in quale zona
    let mutable pressed, tpressed, mpressed = false, false, false
    // posizione inizio in cui e' stato premuto il mouse
    let mutable start = new Point()
    // angolo di rotazione del piano
    let mutable rotate = 0.f
    // valore dello zoom
    let mutable zoomscale = 1.f
    // traslazione del piano
    let mutable translation = new PointF()
    // direzione premuta per scrollare
    let mutable scrolldir = ScrollButton.UP
    // determina il punto da cui iniziare a costruire il prossimo pezzo di strada
    let mutable edge = Point( 450, 300 )
    // determina la direzione della costruzione del circuito
    let mutable direction = "D"
    // determina se il circuito in creazione è completato o no
    let mutable start, curva = false, false
    // la macchina che percorrerà il circuito
    let mutable car = new Car.Car( Position = PointF( 450.f, 300.f ), index = 0 )
    // il punto di partenza della macchina
    let mutable via = Point( 450, 300 )
    // lunghezza di un tratto
    let dist = 20
    // variabili relative alla dimensione del rettangolo di spostamento
    let mutable maxX, maxY, minX, minY = 450, 300, 900, 600
    // varibili relative allo spostamento della pista
    let mutable inizio, fine = PointF(), PointF()
    // determina se stiamo spostando il tracciato
    let mutable move = false
    // variabili relative allo spostamento del rettangolo
    let mutable temp, shift = 0, 0
    // indica il pulsante della manipolazione che e' stato premuto
    let mutable indMani = -1

    // lista dei bottoni di toolbar
    let toolbar = [|
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.ORIZZ, Area = new Rectangle( 0, 0, btnw, btnh ), Index = 0 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.VERT, Area = new Rectangle( 1 * btnw, 0, btnw, btnh ), Index = 1 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.CURVSB, Area = new Rectangle( 2 * btnw, 0, btnw, btnh ), Index = 2 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.CURVSA, Area = new Rectangle( 3 * btnw, 0, btnw, btnh ), Index = 3 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.CURVDA, Area = new Rectangle( 4 * btnw, 0, btnw, btnh ), Index = 4 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.CURVBD, Area = new Rectangle( 5 * btnw, 0, btnw, btnh ), Index = 5 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.UNDO, Area = new Rectangle( 7 * btnw, 0, btnw, btnh ), Index = 6 )
            new ToolbarButton.ToolbarButton( Type = ToolbarButton.START, Area = new Rectangle( 9 * btnw, 0, btnw, btnh ), Index = 7 )
      |]

    // lista dei bottoni di manipolazione
    let manipolation = [|
            new ManipolationButton.ManipolationButton( Type = ManipolationButton.ROTATE_CLOCKWISE, Area = new Rectangle( 0, 4 * btnh, btnw, btnh ), Index = 0 )
            new ManipolationButton.ManipolationButton( Type = ManipolationButton.ROTATE_COUNTERCLOCKWISE, Area = new Rectangle( 0, 5 * btnh, btnw, btnh ), Index = 1 )
            new ManipolationButton.ManipolationButton( Type = ManipolationButton.ZOOM_MORE, Area = new Rectangle( 0, 2 * btnh, btnw, btnh ), Index = 2 )
            new ManipolationButton.ManipolationButton( Type = ManipolationButton.ZOOM_LESS, Area = new Rectangle( 0, 3 * btnh, btnw, btnh ), Index = 3 )
    |]

    // lista dei bottoni di scroll
    let w, h = this.Width, this.Height
    let scrollbtns = [|
        ScrollButton.ScrollButton( ScrollButton.RIGHT, [| new Point( w - btnw, (h - btnh) / 2 ); new Point( w - btnw, (h + btnh) / 2 ); new Point( w, h / 2 ) |] )
        ScrollButton.ScrollButton( ScrollButton.LEFT, [| new Point( 0, h / 2 ); new Point( btnw, (h + btnh) / 2 ); new Point( btnw, (h - btnh) / 2 ) |] )
        ScrollButton.ScrollButton( ScrollButton.DOWN, [| new Point( (w - btnw) / 2, h - btnh ); new Point( (w + btnw) / 2, h - btnh ); new Point( w / 2, h ) |] )
        ScrollButton.ScrollButton( ScrollButton.UP, [| new Point( (w - btnw) / 2, btnh ); new Point( (w + btnw) / 2, btnh ); new Point( w / 2, 0 ) |] )
      |]
    
    do
      this.DoubleBuffered <- true
      this.SetStyle( ControlStyles.DoubleBuffer ||| ControlStyles.AllPaintingInWmPaint, true )

      timerscroll.Tick.Add( fun _ ->
            // controlla la freccia premuta
            match scrolldir with
                | ScrollButton.LEFT -> translation <- new PointF( translation.X + 10.f, translation.Y )
                | ScrollButton.RIGHT -> translation <- new PointF( translation.X - 10.f, translation.Y )
                | ScrollButton.UP -> translation <- new PointF( translation.X, translation.Y + 10.f )
                | ScrollButton.DOWN -> translation <- new PointF( translation.X, translation.Y - 10.f )

            this.Invalidate()
      )

      toolbar.[1].Active <- false
      toolbar.[4].Active <- false
      toolbar.[5].Active <- false
      toolbar.[6].Active <- false
      toolbar.[7].Active <- false

    let t = new Timer(Interval=33)

    do
      t.Tick.Add(fun _  ->
        this.Invalidate()
      )
      t.Start()

    override this.OnResize e =
        base.OnResize e

        let w, h = this.Width, this.Height
        for b in scrollbtns do
          match b.Direction with
              | ScrollButton.RIGHT -> b.ButtonArea <- [| new Point( w - btnw, (h - btnh) / 2 ); new Point( w - btnw, (h + btnh) / 2 ); new Point( w, h / 2 ) |]
              | ScrollButton.LEFT -> b.ButtonArea <- [| new Point( 0, h / 2 ); new Point( btnw, (h + btnh) / 2 ); new Point( btnw, (h - btnh) / 2 ) |]
              | ScrollButton.DOWN -> b.ButtonArea <- [| new Point( (w - btnw) / 2, h - btnh ); new Point( (w + btnw) / 2, h - btnh ); new Point( w / 2, h ) |]
              | ScrollButton.UP -> b.ButtonArea <- [| new Point( (w - btnw) / 2, btnh ); new Point( (w + btnw) / 2, btnh ); new Point( w / 2, 0 ) |]
        this.Invalidate() 
        
    member this.SetActiveButton() =
        match direction with
            | "A" -> toolbar.[1].Active <- true; toolbar.[2].Active <- true; toolbar.[5].Active <- true; toolbar.[0].Active <- false; toolbar.[3].Active <- false; toolbar.[4].Active <- false;
            | "B" -> toolbar.[1].Active <- true; toolbar.[3].Active <- true; toolbar.[4].Active <- true; toolbar.[0].Active <- false; toolbar.[2].Active <- false; toolbar.[5].Active <- false;
            | "S" -> toolbar.[0].Active <- true; toolbar.[4].Active <- true; toolbar.[5].Active <- true; toolbar.[1].Active <- false; toolbar.[2].Active <- false; toolbar.[3].Active <- false;
            | "D" -> toolbar.[0].Active <- true; toolbar.[2].Active <- true; toolbar.[3].Active <- true; toolbar.[1].Active <- false; toolbar.[4].Active <- false; toolbar.[5].Active <- false;
            | _   -> ()

    /// ruota un punto intorno all'origine degli assi di un angolo teta
    member this.Rotate( x:float32, y:float32, teta:float32 ) =
        // ottiene i radianti dell'angolo di rotazione espresso in sessagesimali
        let angle = teta * single System.Math.PI / 180.f
        // calcola le nuove coordinate
        let X = x * cos( angle ) - y * sin( angle )
        let Y = x * sin( angle ) + y * cos( angle )

        new PointF( X, Y )

    /// trasforma le coordinate mondo in quelle vista
    member this.worldToView( x:float32, y:float32 ) =
        let mutable p = new PointF( x - translation.X, y - translation.Y )
        p <- this.Rotate( p.X, p.Y, -rotate )
        new PointF( p.X / zoomscale, p.Y / zoomscale )
         
    override this.OnMouseDown e =
        base.OnMouseDown e

        let mutable found = false
        
        if e.Button = MouseButtons.Left then
            let mutable insert = false

            let mPoint = this.worldToView( single e.Location.X, single e.Location.Y )

            // controlla se e' stato premuto un cambio giri/macchine
            if not start then
                if mondiale.Count > 0 then
                    let mutable allSetted = true
                    for pista in mondiale do
                        if not found && pista.CheckPressedSettings( mPoint ) then
                            found <- true

                        // controlla se tutti i laps e le macchine sono > 0 per attivare/disattivare lo start
                        if pista.Laps = 0 || pista.Macchine = 0 then
                            allSetted <- false

                    toolbar.[7].Active <- allSetted
            else
                // controlla se e' stato premuto il tasto di chiusura risultati
                let mutable allClosed = true
                for pista in mondiale do
                    if not(pista.EndRace()) || (not( pista.Result.ClosedByHand ) && not( pista.Result.CheckCloseButtonPressed( mPoint ) )) then
                        allClosed <- false
                    else
                        found <- true

                if allClosed then
                   start <- false 
                   toolbar.[7].Active <- true
                else
                    toolbar.[7].Active <- false
                    
            
            for b in toolbar do
                insert <- false

                if not start && not found && b.Index < 7 && b.contains( e.Location ) && b.Active then
                    b.Pressed <- true
                    tpressed <- true
                    found <- true
                    this.Invalidate()

                    if count = 0 then
                        tracks <- tracks + 1
                        mondiale.Add( Pista.Pista( Index = tracks ) )

                    if b.Index = 0 then
                        if direction = "D" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X + dist, edge.Y )
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                        else if direction = "S" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X - dist, edge.Y )
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                    else if b.Index = 1 then
                        if direction = "A" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X, edge.Y - dist )
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                        else if direction = "B" then                            
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X, edge.Y + dist )
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                    else if b.Index = 2 then
                        if direction = "D" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge, Aggiusta = true ) )
                            edge <- Point( edge.X + dist, edge.Y + dist )
                            direction <- "B"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                        else if direction = "A" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X - dist, edge.Y - dist )                          
                            direction <- "S"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                    else if b.Index = 3 then
                        if direction = "D" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge, Aggiusta = true ) )
                            edge <- Point( edge.X + dist, edge.Y - dist )
                            direction <- "A"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                        else if direction = "B" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X - dist, edge.Y + dist )
                            direction <- "S"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                    else if b.Index = 4 then
                        if direction = "S" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge, Aggiusta = true ) )
                            edge <- Point( edge.X - dist, edge.Y - dist )
                            direction <- "A"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                        else if direction = "B" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X + dist, edge.Y + dist )
                            direction <- "D"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                    else if b.Index = 5 then
                        if direction = "A" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge, Aggiusta = true ) )
                            edge <- Point( edge.X + dist, edge.Y - dist )
                            direction <- "D"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                        else if direction = "S" then
                            mondiale.Item( tracks ).Add <- ( Strada.Strada( b.Index, direction, Position = edge ) )
                            edge <- Point( edge.X - dist, edge.Y + dist )
                            direction <- "B"
                            insert <- true
                            toolbar.[6].Active <- true
                            this.SetActiveButton()
                    else if b.Index = 6 && tracks >= 0 then // tasto undo
                        let last = mondiale.Item( tracks ).RemoveLast()
                        if last <> null then
                            edge <- last.Position
                            direction <- last.Direction
                            this.SetActiveButton()
                            if mondiale.Item( tracks ).Count = 0 then
                                toolbar.[6].Active <- false
                        else
                            // rimuove il tracciato perche' non ci sono piu' blocchi
                            mondiale.RemoveAt( tracks ) |> ignore
                            tracks <- tracks - 1
                    
                    maxX <- max maxX edge.X
                    maxY <- max maxY edge.Y
                    minX <- min minX edge.X
                    minY <- min minY edge.Y

                    if insert then
                        count <- count + 1

                        via <- mondiale.Item( tracks ).GetCircuit.Item( 0 ).Position
                        if edge = via && direction = "D" then
                            let m = mondiale.Item( tracks )
                            m.Completed <- true
                            m.GetCircuit.Item( 0 ).Traguardo <- true

                            minY <- minY - 5
                            minX <- minX - 5

                            m.maxX <- maxX
                            m.maxY <- maxY
                            m.minY <- minY
                            m.minX <- minX

                            m.AddBlock( PointF( single maxX + 10.f, single maxY + 10.f ), 0 )
                            m.AddBlock( PointF( single maxX + 10.f, single maxY + 55.f ), 1 )
                            m.AddMod( PointF( single maxX + 40.f, single maxY + 10.f ), 0 )
                            m.AddMod( PointF( single maxX + 40.f, single maxY + 22.f ), 1 )
                            m.AddMod( PointF( single maxX + 40.f, single maxY + 55.f ), 0 )
                            m.AddMod( PointF( single maxX + 40.f, single maxY + 67.f ), 1 )

                            m.Handle <- RectangleF( single minX, single minY, single( maxX - minX + 50 ), single( maxY - minY + 79 ) )

                            m.maxX <- maxX + 50
                            m.maxY <- maxY + 79

                            count <- 0
                            maxX <- 450
                            maxY <- 300
                            minX <- 900
                            minY <- 600
                            toolbar.[6].Active <- false
                
                // se si vuole iniziare a far girare le vetture nei tracciati (solo se completati tutti)
                else if not found && b.Index = 7 && b.contains( e.Location ) && not start then
                    let mutable toStart = true
                    for m in mondiale do
                        if m.Completed = false then
                            toStart <- false
                    
                    if toStart then
                        for m in mondiale do
                            m.Start( m.Index, m.GetCircuit.Item( 0 ).Position.X, m.GetCircuit.Item( 0 ).Position.Y, m.Laps )
                        start <- true
                        toolbar.[7].Active <- false

                    b.Pressed <- true
                    tpressed <- true
                    found <- true
                    this.Invalidate()

                // se e' stato premuto nel rettangolo di spostamento circuito
                else if not found && not start then
                    for m in mondiale do
                        if m.Contains( mPoint ) then
                            found <- true
                            tpressed <- true
                            inizio <- this.worldToView( single e.Location.X, single e.Location.Y )
                            fine <- PointF( 0.f, 0.f )
                            move <- true
                            temp <- m.Index
                            this.Invalidate()

        // controlla se e' stato premuto un tasto della manipolation
        if not found && e.Button = MouseButtons.Left then
            for m in manipolation do
                if not found && m.contains( e.Location ) then
                    indMani <- m.Index
                    mpressed <- true
                    m.Pressed <- true
                    found <- true
                    this.Invalidate()

        // controlla se e' stato premuta una freccia direzionale
        if not found && e.Button = MouseButtons.Left then
            for sb in scrollbtns do
                if sb.contains( e.Location ) then
                    scrolldir <- sb.Direction
                    timerscroll.Start()
                    found <- true
                    this.Invalidate()
                else if sb.contains( e.Location ) then
                    scrolldir <- sb.Direction
                    timerscroll.Start()
                    found <- true
                    this.Invalidate()

    override this.OnMouseMove e =
        base.OnMouseMove e

        if tpressed && move then
            let m = mondiale.Item( temp )

            if shift = 0 then
                fine <- this.worldToView( single e.Location.X, single e.Location.Y )
                m.Handle <- RectangleF( m.Handle.X + single( fine.X - inizio.X ), m.Handle.Y + single( fine.Y - inizio.Y ), m.Handle.Width, m.Handle.Height )
                shift <- shift + 1
            else
                let mPoint = this.worldToView( single e.Location.X, single e.Location.Y )
                let diffX, diffY = (mPoint.X - fine.X), (mPoint.Y - fine.Y)
                m.Handle <- RectangleF( m.Handle.X + single diffX, m.Handle.Y + single diffY, m.Handle.Width, m.Handle.Height )
                fine <- mPoint

    override this.OnMouseUp e =
        base.OnMouseUp e

        if timerscroll.Enabled then
            timerscroll.Stop()
            
        if tpressed then
            if move then
                fine <- this.worldToView( single e.Location.X, single e.Location.Y )
                move <- false
                let m = mondiale.Item( temp )

                for s in m.GetCircuit do
                    if s.Modified then
                        s.Aggiusta <- true
                    s.Position <- Point( s.Position.X + int( fine.X - inizio.X ), s.Position.Y + int( fine.Y - inizio.Y ) )
                for b in m.GetBlocks do
                    b.Position <- PointF( b.Position.X + single (fine.X - inizio.X), b.Position.Y + single (fine.Y - inizio.Y) )
                for l in m.GetMod do
                    l.Position <- PointF( l.Position.X + single (fine.X - inizio.X), l.Position.Y + single (fine.Y - inizio.Y) )
                m.maxX <- m.maxX + int( fine.X - inizio.X )
                m.minX <- m.minX + int( fine.X - inizio.X )
                m.maxY <- m.maxY + int( fine.Y - inizio.Y )
                m.minY <- m.minY + int( fine.Y - inizio.Y )

                temp <- -1
                shift <- 0

            for t in toolbar do
                t.Pressed <- false
                this.Invalidate()

            tpressed <- false

        else if mpressed then
            for m in manipolation do
                m.Pressed <- false

            if indMani = 0 then
                rotate <- rotate + 5.f
            else if indMani = 1 then
                rotate <- rotate - 5.f
            else if indMani = 2 then
                zoomscale <- zoomscale * 1.1f
            else if indMani = 3 then
                zoomscale <- zoomscale / 1.1f

            this.Invalidate()
            indMani <- - 1

            mpressed <- false

    override this.OnPaint e =
        base.OnPaint e

        let g = e.Graphics
        g.SmoothingMode <- Drawing2D.SmoothingMode.HighQuality
        g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
        
        // trasla il piano
        g.TranslateTransform( translation.X, translation.Y )
        g.RotateTransform( rotate )
        g.ScaleTransform( zoomscale, zoomscale )
        
        for m in mondiale do
            m.Paint( g, this.Font )

        // ripristina le coordinate originali
        g.ResetTransform()
                
        for b in toolbar do
            b.paint( g, this.BackColor )

        for b in manipolation do
            b.paint( g, this.BackColor )

        for sb in scrollbtns do
            sb.paint( g )

let m = new Main( Dock = DockStyle.Fill )

f.Controls.Add( m )

f.Show()
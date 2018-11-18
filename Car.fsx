open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

#load "Strada.fsx"

type Car( index: int ) =
    //
    let mutable position = PointF()
    // raggio del cerchio
    let ray = 7
    // determina quale tratto di strada sta percorrendo
    let mutable tratto = 0
    // determina se la macchina sta curvando oppure no
    let mutable curva = false
    // la posizione della vettura sull'asse y
    let mutable y = 0.f
    // il numero di giri da compiere
    let mutable laps = 0
    // indica il numero di macchina nel circuito
    let mutable number = 0
    // indica se la vettura ha terminato i giri
    let mutable finish = false
    //la velocita' della vettura
    let mutable speed, basic = 1.f, 1.f
    //
    let mutable posx, posy, _pos = 0, 0, 0
    // lunghezza di un tratto
    let dist = 20

    member this.PosX
        with get() = posx
        and set( v ) = posx <- v

    member this.PosY
        with get() = posy
        and set( v ) = posy <- v

    member this.GetArea() =
        let gp = new GraphicsPath()
        gp.AddEllipse( RectangleF( position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray ) )
        gp

    member this.Pos
        with get() = _pos
        and set( v ) = _pos <- v

    ///indica il numero della macchina nel circuito
    member this.Number
        with get() = number
        and set( v ) = number <- v

    ///restituisce la velocita' di spostamento della vettura
    member this.Speed
        with get() = speed
        and set( v ) = speed <- v
    
    ///indica la posizione della vettura
    member this.Position
        with get() = position
        and set( v ) = position <- v

    ///indica quale tratto di strada sta attraversando
    member this.Tratto
        with get() = tratto
        and set( v ) = tratto <- v

    ///indica in quale circuito gareggia
    member this.Tracciato
        with get() = index

    ///indica il numero di giri che deve ancora compiere
    member this.Laps
        with get() = laps
        and set( v ) = laps <- v

    ///determina se la macchina sta eseguendo o no una curva
    member this.Curva
        with get() = curva
        and set( v ) = curva <- v

    ///determina la posizione y della macchina al momento dell'ingresso in curva
    member this.Begin
        with get() = y
        and set( v ) = y <- v

    ///determina se la vettura ha terminato i giri da compiere
    member this.Finished
        with get() = finish
        and set( v ) = finish <- v

    ///restituisce la velocita' corretta della vettura
    member this.Basic
        with get() = basic
        and set( v ) = basic <- v

    member this.Paint( g: Graphics ) =
        if number = 0 then
            g.FillEllipse( Brushes.Orange, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
            g.DrawEllipse( Pens.Black, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
        else if number = 1 then
            g.FillEllipse( Brushes.Green, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
            g.DrawEllipse( Pens.Black, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
        else if number = 2 then
            g.FillEllipse( Brushes.White, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
            g.DrawEllipse( Pens.Black, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
        else if number = 3 then
            g.FillEllipse( Brushes.Yellow, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )
            g.DrawEllipse( Pens.Black, position.X - single ray / 2.f, position.Y - single ray / 2.f, single ray, single ray )

    member this.updateCar( car: Car, strada:Strada.Strada, numTratti:int, pista:ResizeArray<Strada.Strada> ) =
        // raggio al quadrato
        let rayquad = 400
        let maxStep = 20
        if tratto < numTratti then
            if strada.Tipo = 0 then
                if strada.Direction = "D" then
                    position <- PointF( position.X + speed, position.Y )
                if int position.X >= strada.Position.X + 20 then
                    tratto <- tratto + 1

                else if strada.Direction = "S" then
                    position <- PointF( position.X - speed, position.Y )
                if int position.X <= strada.Position.X - 20 then
                    tratto <- tratto + 1

            else if strada.Tipo = 1 then
                if strada.Direction = "A" then
                    position <- PointF( position.X, position.Y - speed )
                if int position.Y <= strada.Position.Y - 20 then
                    tratto <- tratto + 1

                else if strada.Direction = "B" then
                    position <- PointF( position.X, position.Y + speed )
                if int position.Y >= strada.Position.Y + 20 then
                    tratto <- tratto + 1

            else if strada.Tipo = 2 then
                if not curva then
                    _pos <- 0
                    posx <- int position.X
                    posy <- int position.Y
                    y <- position.Y
                    curva <- true
                    speed <- 1.f
                if strada.Direction = "A" then
                    let x = sqrt( single rayquad - ( y - single( strada.Position.Y ) ) * ( y - single( strada.Position.Y ) ) ) + single( strada.Position.X - 20 )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx - 20.f, single posy - 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y - speed

                else if strada.Direction = "D" then
                    let x = sqrt( single rayquad - ( y - single( strada.Position.Y + 20 ) ) * ( y - single( strada.Position.Y + 20 ) ) ) + single( strada.Position.X )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx + 20.f, single posy + 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y + speed

            else if strada.Tipo = 3 then
                if not curva then
                    _pos <- 0
                    posx <- int position.X
                    posy <- int position.Y
                    y <- position.Y
                    curva <- true
                    speed <- 1.f
                if strada.Direction = "D" then
                    let x = (single (strada.Position.X) + (sqrt( single rayquad - ( y - single (strada.Position.Y - dist ) ) * ( y - single (strada.Position.Y - dist ) ) ) ) )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx + 20.f, single posy - 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y - speed
                else if strada.Direction = "B" then
                    let x = sqrt( single rayquad - ( y - single( strada.Position.Y ) ) * ( y - single( strada.Position.Y ) ) ) + single( strada.Position.X - 20 )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx - 20.f, single posy + 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y + speed

            else if strada.Tipo = 4 then
                if not curva then
                    _pos <- 0
                    posx <- int position.X
                    posy <- int position.Y
                    y <- position.Y
                    curva <- true
                    speed <- 1.f
                if strada.Direction = "S" then
                    let x = single( strada.Position.X ) - sqrt( single rayquad - ( y - single( strada.Position.Y - 20 ) ) * ( y - single( strada.Position.Y - 20 ) ) )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx - 20.f, single posy - 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y - speed
                else if strada.Direction = "B" then
                    let x = single( strada.Position.X + 20 ) - sqrt( single rayquad - ( y - single( strada.Position.Y ) ) * ( y - single( strada.Position.Y ) ) )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx + 20.f, single posy + 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y + speed

            else if strada.Tipo = 5 then
                if not curva then
                    _pos <- 0
                    posx <- int position.X
                    posy <- int position.Y
                    y <- position.Y
                    curva <- true
                    speed <- 1.f
                if strada.Direction = "S" then
                    let x = single( strada.Position.X ) - sqrt( single rayquad - ( y - single( strada.Position.Y + 20 ) ) * ( y - single( strada.Position.Y + 20 ) ) )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx - 20.f, single posy + 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y + speed
                else if strada.Direction = "A" then
                    let x = single( strada.Position.X + 20 ) - sqrt( single rayquad - single( y - single( strada.Position.Y ) ) * single( y - single( strada.Position.Y ) ) )
                    position <- PointF( x, y )
                    _pos <- _pos + 1

                    if _pos = maxStep then
                        position <- PointF( single posx + 20.f, single posy - 20.f )
                        curva <- false
                        speed <- basic
                        tratto <- tratto + 1
                    else
                        y <- y - speed
            
            if tratto = numTratti then
                laps <- laps - 1
                if laps > 0 then
                    tratto <- 0
                else
                    finish <- true
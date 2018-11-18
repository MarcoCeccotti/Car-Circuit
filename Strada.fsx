open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

[<AllowNullLiteralAttribute>]
type Strada( tipo: int, direction: string ) =

    let mutable start, copy = Point(), Point()
    let mutable traguardo, aggiusta, modificata = false, false, false
    
    let d = 5

    member this.Tipo
        with get() = tipo

    member this.Direction
        with get() = direction

    member this.Position
        with get() = start
        and set( v ) = start <- v
                       copy <- v

    member this.Traguardo
        with get() = traguardo
        and set( v ) = traguardo <- v

    ///restituisce il valore di aggiustamento curva per il rendering
    member this.Aggiusta
        with set( v ) = aggiusta <- v

    ///restituisce il valore di modifica curva per il rendering
    member this.Modified
        with get() = modificata

    member this.paint( g:Graphics ) =
        
        use p = new Pen( Color.Black, 2.f )
        
        if tipo = 0 then
            if direction = "D" then
                g.FillRectangle( Brushes.Gray, Rectangle( Point( copy.X, copy.Y - d ), Size( 20, 10 ) ) )
                g.DrawLine( p, Point( copy.X, copy.Y - d ), Point( copy.X + 20, copy.Y - d ) )
                g.DrawLine( p, Point( copy.X, copy.Y + d ), Point( copy.X + 20, copy.Y + d ) )
            else // "S"
                g.FillRectangle( Brushes.Gray, Rectangle( Point( copy.X - 20, copy.Y - d ), Size( 20, 10 ) ) )
                g.DrawLine( p, Point( copy.X, copy.Y - d ), Point( copy.X - 20, copy.Y - d ) )
                g.DrawLine( p, Point( copy.X, copy.Y + d ), Point( copy.X - 20, copy.Y + d ) )

        else if tipo = 1 then
            if direction = "A" then
                g.FillRectangle( Brushes.Gray, Rectangle( Point( copy.X - d, copy.Y - 20 ), Size( 10, 20 ) ) )
                g.DrawLine( p, Point( copy.X - d, copy.Y ), Point( copy.X - d, copy.Y - 20 ) )
                g.DrawLine( p, Point( copy.X + d, copy.Y ), Point( copy.X + d, copy.Y - 20 ) )
            else // "B"
                g.FillRectangle( Brushes.Gray, Rectangle( Point( copy.X - d, copy.Y ), Size( 10, 20 ) ) )
                g.DrawLine( p, Point( copy.X - d, copy.Y ), Point( copy.X - d, copy.Y + 20 ) )
                g.DrawLine( p, Point( copy.X + d, copy.Y ), Point( copy.X + d, copy.Y + 20 ) )

        else if tipo = 2 then
            if aggiusta then
                copy.X <- copy.X + 20
                copy.Y <- copy.Y + 20
                aggiusta <- false
                modificata <- true

            use gp = new GraphicsPath()
            gp.AddCurve( [| Point( copy.X + d, copy.Y ); Point( copy.X + d - 1, copy.Y - 7 ); Point( copy.X + d - 5, copy.Y - 15 ); Point( copy.X + d - 10, copy.Y - 20 ); Point( copy.X + d - 18, copy.Y - 24 ); Point( copy.X + d - 25, copy.Y - 25 ) |] )
            gp.AddBezier( Point( copy.X - d - 15, copy.Y - 15 ), Point( copy.X - d - 6, copy.Y - 12 ), Point( copy.X - d - 3, copy.Y - 9 ), Point( copy.X - d, copy.Y ) )
            g.FillPolygon( Brushes.Gray, gp.PathPoints )

            g.DrawCurve( p, [| Point( copy.X + d, copy.Y ); Point( copy.X + d - 1, copy.Y - 7 ); Point( copy.X + d - 5, copy.Y - 15 ); Point( copy.X + d - 10, copy.Y - 20 ); Point( copy.X + d - 18, copy.Y - 24 ); Point( copy.X + d - 25, copy.Y - 25 ) |] )
            g.DrawBezier( p, Point( copy.X - d, copy.Y ), Point( copy.X - d - 3, copy.Y - 9 ), Point( copy.X - d - 6, copy.Y - 12 ), Point( copy.X - d - 15, copy.Y - 15 ) )
                
            gp.Dispose()

        else if tipo = 3 then
            if aggiusta then
                copy.X <- copy.X + 20
                copy.Y <- copy.Y - 20
                aggiusta <- false
                modificata <- true

            use gp = new GraphicsPath()
            gp.AddCurve( [| Point( copy.X + d, copy.Y ); Point( copy.X + d - 1, copy.Y + 7 ); Point( copy.X + d - 5, copy.Y + 15 ); Point( copy.X + d - 10, copy.Y + 20 ); Point( copy.X + d - 18, copy.Y + 24 ); Point( copy.X + d - 25, copy.Y + 25 ) |] )
            gp.AddBezier( Point( copy.X - d - 15, copy.Y + 15 ), Point( copy.X - d - 6, copy.Y + 12 ), Point( copy.X - d - 3, copy.Y + 9 ), Point( copy.X - d, copy.Y ) )
            g.FillPolygon( Brushes.Gray, gp.PathPoints )

            g.DrawCurve( p, [| Point( copy.X + d, copy.Y ); Point( copy.X + d - 1, copy.Y + 7 ); Point( copy.X + d - 5, copy.Y + 15 ); Point( copy.X + d - 10, copy.Y + 20 ); Point( copy.X + d - 18, copy.Y + 24 ); Point( copy.X + d - 25, copy.Y + 25 ) |] )
            g.DrawBezier( p, Point( copy.X - d, copy.Y ), Point( copy.X - d - 3, copy.Y + 9 ), Point( copy.X - d - 6, copy.Y + 12 ), Point( copy.X - d - 15, copy.Y + 15 ) )
                
            gp.Dispose()                

        else if tipo = 4 then
            if aggiusta then
                copy.X <- copy.X - 20
                copy.Y <- copy.Y - 20
                aggiusta <- false
                modificata <- true

            use gp = new GraphicsPath()
            gp.AddCurve( [| Point( copy.X - d, copy.Y ); Point( copy.X - d + 1, copy.Y + 7 ); Point( copy.X - d + 5, copy.Y + 15 ); Point( copy.X - d + 10, copy.Y + 20 ); Point( copy.X - d + 18, copy.Y + 24 ); Point( copy.X - d + 25, copy.Y + 25 ) |] )
            gp.AddBezier( Point( copy.X + d + 15, copy.Y + 15 ), Point( copy.X + d + 6, copy.Y + 12 ), Point( copy.X + d + 3, copy.Y + 9 ), Point( copy.X + d, copy.Y ) )
            g.FillPolygon( Brushes.Gray, gp.PathPoints )
                
            g.DrawCurve( p, [| Point( copy.X - d, copy.Y ); Point( copy.X - d + 1, copy.Y + 7 ); Point( copy.X - d + 5, copy.Y + 15 ); Point( copy.X - d + 10, copy.Y + 20 ); Point( copy.X - d + 18, copy.Y + 24 ); Point( copy.X - d + 25, copy.Y + 25 ) |] )
            g.DrawBezier( p, Point( copy.X + d, copy.Y ), Point( copy.X + d + 3, copy.Y + 9 ), Point( copy.X + d + 6, copy.Y + 12 ), Point( copy.X + d + 15, copy.Y + 15 ) )
                
            gp.Dispose()

        else if tipo = 5 then
            if aggiusta then
                copy.X <- copy.X + 20
                copy.Y <- copy.Y - 20
                aggiusta <- false
                modificata <- true

            use gp = new GraphicsPath()
            gp.AddCurve( [| Point( copy.X, copy.Y - d ); Point( copy.X - 7, copy.Y - d + 1 ); Point( copy.X - 15, copy.Y - d + 5 ); Point( copy.X - 20, copy.Y - d + 10 ); Point( copy.X - 24, copy.Y - d + 18 ); Point( copy.X - 25, copy.Y - d + 25 ) |] )
            gp.AddBezier( Point( copy.X - 15, copy.Y + d + 15 ), Point( copy.X - 12, copy.Y + d + 6 ), Point( copy.X - 9, copy.Y + d + 3 ), Point( copy.X, copy.Y + d ) )
            g.FillPolygon( Brushes.Gray, gp.PathPoints )
                
            g.DrawCurve( p, [| Point( copy.X, copy.Y - d ); Point( copy.X - 7, copy.Y - d + 1 ); Point( copy.X - 15, copy.Y - d + 5 ); Point( copy.X - 20, copy.Y - d + 10 ); Point( copy.X - 24, copy.Y - d + 18 ); Point( copy.X - 25, copy.Y - d + 25 ) |] )
            g.DrawBezier( p, Point( copy.X, copy.Y + d ), Point( copy.X - 9, copy.Y + d + 3 ), Point( copy.X - 12, copy.Y + d + 6 ), Point( copy.X - 15, copy.Y + d + 15 ) )
                
            gp.Dispose()
        
        let size = 3
        if traguardo then
            g.DrawRectangles( Pens.White, [| Rectangle( start.X, start.Y - size, size, size ); Rectangle( start.X + size, start.Y - 2 * size, size, size ); Rectangle( start.X, start.Y - 3 * size, size, size ); Rectangle( start.X + size, start.Y, size, size ); Rectangle( start.X, start.Y + size, size, size ) |] )
            g.FillRectangles( Brushes.White, [| Rectangle( start.X, start.Y - size, size, size ); Rectangle( start.X + size, start.Y - 2 * size, size, size ); Rectangle( start.X, start.Y - 3 * size, size, size ); Rectangle( start.X + size, start.Y, size, size ); Rectangle( start.X, start.Y + size, size, size ) |] )

            g.DrawRectangles( Pens.Black, [| Rectangle( start.X + size, start.Y - size, size, size ); Rectangle( start.X, start.Y - 2 * size, size, size ); Rectangle( start.X, start.Y, size, size ); Rectangle( start.X + size, start.Y + size, size, size ) |] )
            g.FillRectangles( Brushes.Black, [| Rectangle( start.X + size, start.Y - size, size, size ); Rectangle( start.X, start.Y - 2 * size, size, size ); Rectangle( start.X, start.Y, size, size ); Rectangle( start.X + size, start.Y + size, size, size ) |] )
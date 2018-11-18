open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

type Mod() =
    
    let mutable position = PointF()
    let mutable index = 0

    member this.Position
        with get() = position
        and set( v ) = position <- v

    member this.Index
        with get() = index
        and set( v ) = index <- v

    member this.Contains( e:PointF ) =
        let rect = RectangleF( position, SizeF( 10.f, 12.f ) )
        if rect.Contains( e ) then
            true
        else
            false

    member this.Paint( g:Graphics ) =
        g.DrawRectangle( Pens.Black, Rectangle( int position.X, int position.Y, 10, 12 ) )
        if index = 0 then
            g.DrawLine( Pens.Black, Point( int position.X + 3, int position.Y + 6 ), Point( int position.X + 7, int position.Y + 6 ) )
            g.DrawLine( Pens.Black, Point( int position.X + 5, int position.Y + 4 ), Point( int position.X + 5, int position.Y + 8 ) )
        else
            g.DrawLine( Pens.Black, Point( int position.X + 3, int position.Y + 6 ), Point( int position.X + 7, int position.Y + 6 ) )

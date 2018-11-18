open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

type Block() =
    
    let mutable position = PointF()
    let mutable index  = 0

    member this.Position
        with get() = position
        and set( v ) = position <- v

    member this.Index
        with get() = index
        and set( v ) = index <- v

    member this.Contains( e:PointF ) =
        let rect = RectangleF( position, SizeF( 40.f, 24.f ) )
        if rect.Contains( e ) then
            true
        else
            false

    member this.Paint( g:Graphics ) =
        g.DrawRectangle( Pens.Black, Rectangle( int position.X, int position.Y, 40, 24 ) )

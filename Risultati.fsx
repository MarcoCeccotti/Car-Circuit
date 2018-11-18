open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

#load "Car.fsx"

type Risultati() =
    inherit UserControl()

    // rende visibile il tabellone dei vincitori
    let mutable visible = false
    // dimensioni tabellone risultati
    let width, height = 80.f, 90.f
    // dimensioni tasto di chiusura
    let w, h = 15.f, 15.f
    // bottone di chiusura
    let mutable close = None
    //
    let mutable closedByHand = false

    ///
    member this.Visible
        with get() = visible
        and set( v ) =
            visible <- v
            closedByHand <- not visible

    ///
    member this.ClosedByHand
        with get() = closedByHand

    /// 
    member this.CheckCloseButtonPressed( p:PointF ) =
        match close with
            | None -> false
            | Some( v:RectangleF ) ->
                    let cont = v.Contains( p )
                    if cont then
                        closedByHand <- true
                        visible <- false
                    cont

    /// 
    member private this.DrawCloseButton( g:Graphics, v:RectangleF ) =
        g.FillRectangle( Brushes.Red, v )
        g.DrawRectangle( Pens.Black, v.X, v.Y, v.Width, v.Height )

    /// 
    member this.Paint( g:Graphics, auto: ResizeArray<Car.Car>, index: int, pos:PointF, size:SizeF ) =
        let rect = new RectangleF( pos.X + size.Width / 2.f - (width / 2.f), pos.Y + size.Height / 2.f - (height / 2.f), width, height )

        g.FillRectangle( Brushes.White, rect )
        g.DrawRectangle( Pens.Black, rect.X, rect.Y, rect.Width, rect.Height )

        let mutable offsetX, offsetY = 5.f, 5.f

        let str = "ARRIVO"
        let strSize = g.MeasureString( str, this.Font )
        g.DrawString( str, this.Font, Brushes.Black, rect.X + rect.Width / 2.f - (strSize.Width / 2.f), rect.Y + offsetY )

        let brushes = [| Brushes.Orange; Brushes.Green; Brushes.White; Brushes.Yellow |]
        let ballWidth, ballHeight = 10.f, 10.f
            
        offsetY <- g.MeasureString( "1°", this.Font ).Height + offsetY * 2.5f
        for i in 1 .. auto.Count do
            g.DrawString( sprintf "%d°" i, this.Font, Brushes.Black, rect.X + offsetX, rect.Y + offsetY )
            
            // disegna la pallina
            g.DrawEllipse( Pens.Black, rect.X + offsetX * 5.f, rect.Y + offsetY, ballWidth, ballHeight )
            g.FillEllipse( brushes.[i - 1], RectangleF( rect.X + offsetX * 5.f, rect.Y + offsetY, ballWidth, ballHeight ) )

            offsetY <- offsetY + 2.f + strSize.Height

        // disegna il rettangolo di chiusura
        let closeButton = new RectangleF( rect.X + rect.Width - w, rect.Y, w, h )
        close <- Some( closeButton )
        this.DrawCloseButton( g, closeButton )
open System.Drawing
open System.Drawing.Drawing2D

// tipi di bottone
type Type =
  | ORIZZ
  | VERT
  | CURVSB
  | CURVSA
  | CURVBD
  | CURVDA
  | START
  | UNDO

// toolbar dei tasti per lo zoom e l'animazione
type ToolbarButton() =
  // area del bottone
  let mutable rect = new Rectangle()
  // nome del bottone
  let mutable name = ORIZZ
  // determina se il bottone e' stato premuto
  let mutable pressed = false
  // determina se l'animazione e' stata attivata
  let mutable anim = false
  // posizione nella toolbar
  let mutable index = 0
  // determina se il bottone e' attivo
  let mutable active = true

  let clickEvt = new Event<System.EventArgs>()
  member this.Click = clickEvt.Publish

  member this.Index
    with get() = index
    and set( v ) =
            index <- v

  member this.Active
    with get() = active
    and set( v ) =
            active <- v

  member this.Type
    with get() = name
    and set( n ) =
            name <- n

  member this.Area
    with get() = rect
    and set( r ) =
            rect <- r

  member this.Pressed
    with get() = pressed
    and set( v ) =
            if not v || active then
                pressed <- v

  member this.contains( p:Point ) =
    rect.Contains( p )

  member this.paint( g:Graphics, back:Color ) =
    let l, t, w, h = rect.Left, rect.Top, rect.Width, rect.Height
    use b = new SolidBrush( if active then back else Color.Gray )
    g.FillRectangle( b, rect )
    use pen = new Pen( Color.Black, 2.f )
    let offset = if pressed then 2 else 0
    use p = new Pen( Color.Blue, 2.f )
    match name with
        | ORIZZ ->
            g.DrawLine( p, l + w/4, t + h/2, l + w * 3/4, t + h/2 )
        | VERT ->
            g.DrawLine( p, l + w/2, t + h/4, l + w/2, t + h * 3/4 )
        | CURVSB ->
            g.DrawBezier( p, Point( l + w/4, t + h/4 ), Point( l + w/4 + 12, t + h/4 + 4 ), Point( l + w/4 + 16, t + h/4 + 8 ), Point( l + w * 3/4, t + h * 3/4 ) )
        | CURVSA ->
            g.DrawBezier( p, Point( l + w/4, t + h * 3/4 ), Point( l + w/4 + 12, t + h * 3/4 - 4 ), Point( l + w/4 + 16, t + h * 3/4 - 8 ), Point( l + w * 3/4, t + h/4 ) )
        | CURVDA ->
            g.DrawBezier( p, Point( l + w * 3/4, t + h * 3/4 ), Point( l + w * 3/4 - 12, t + h * 3/4 - 4 ), Point( l + w * 3/4 - 16, t + h * 3/4 - 8 ), Point( l + w/4, t + h/4 ) )
        | CURVBD ->
            g.DrawCurve( p, [| Point( l + w/4, t + h * 3/4 ); Point( l + w/2, t + h * 4/10 ); Point( l + w * 3/4, t + h/4 ) |], 1.f )
        | UNDO ->
            use gp = new GraphicsPath()
            gp.AddPolygon( [| Point( l + w/6 + offset, t + h/2 + offset );
                              Point( l + w/2 + offset, t + h/6 + offset );
                              Point( l + w/2 + offset, t + h/3 + offset );
                              Point( l + w * 5/6 + offset, t + h/3 + offset );
                              Point( l + w * 5/6 + offset, t + h * 2/3 + offset );
                              Point( l + w/2 + offset, t + h * 2/3 + offset );
                              Point( l + w/2 + offset, t + h * 5/6 + offset ) |] )

            g.FillPolygon( Brushes.Red, gp.PathPoints )
            g.DrawPolygon( Pens.Black, gp.PathPoints )
        | START ->
            g.FillPolygon( Brushes.Green, [| Point( l + w/4 + offset, t + h/4 + offset );
                                             Point( l + w/4 + offset, t + h * 3/4 + offset );
                                             Point( l + w * 3/4 + offset, t + h/2 + offset ) |] )

    use dd = new Pen( SystemColors.ControlDarkDark )
    use d = new Pen( SystemColors.ControlDark )
    use lll = new Pen( SystemColors.ControlLightLight )
    use ll = new Pen( SystemColors.ControlLight )

    let ul1, ul2, br1, br2 =
      if pressed then 
        (dd, d, ll, lll)
      else
        (lll, ll, d, dd)

    g.DrawLine( ul1, l, t, l + w, t )
    g.DrawLine( ul1, l, t, l, t + h )
    g.DrawLine( ul2, l + 1, t + 1, l + w, t + 1 )
    g.DrawLine( ul2, l + 1, t + 1, l + 1, t + h )
    g.DrawLine( br1, l + 2, t + h - 2, l + w, t + h - 2 )
    g.DrawLine( br1, l + w - 2, t + 1, l + w - 2, t + h - 2 )
    g.DrawLine( br2, l + w - 1, t, l + w - 1, t + h - 1 )
    g.DrawLine( br2, l + 1, t + h - 1, l + w - 1, t + h - 1 )
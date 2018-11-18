open System.Drawing

// tipi di bottone
type Type =
  | ROTATE_COUNTERCLOCKWISE
  | ROTATE_CLOCKWISE
  | ZOOM_MORE
  | ZOOM_LESS

// toolbar dei tasti per lo zoom e l'animazione
type ManipolationButton() =
  // area del bottone
  let mutable rect = new Rectangle()
  // nome del bottone
  let mutable name = ZOOM_MORE   
  // determina se il bottone e' stato premuto
  let mutable pressed = false
  // determina se l'animazione e' stata attivata
  let mutable anim = false
  // posizione nella manipolation
  let mutable index = 0

  let clickEvt = new Event<System.EventArgs>()

  member this.Click = clickEvt.Publish
  
  member this.Index
    with get() = index
    and set( v ) =
            index <- v

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
            pressed <- v

  member this.contains( p:Point ) =
    let cont = rect.Contains( p )
    if cont then
        pressed <- not pressed
        // esegue l'evento associato al pulsante
        if not pressed then
            clickEvt.Trigger( new System.EventArgs() )
    cont

  member this.paint( g:Graphics, back:Color ) =
    let l, t, w, h = rect.Left, rect.Top, rect.Width, rect.Height
    use b = new SolidBrush( back )
    g.FillRectangle( b, rect )
    use pen = new Pen( Color.Black, 2.f )
    let offset = if pressed then 2 else 0
    match name with
        | ZOOM_MORE ->
            g.DrawLine( pen, l + w/4 + offset, t + h/2 + offset, l + w * 3/4 + offset, t + h/2 + offset )
            g.DrawLine( pen, l + w/2 + offset, t + h/4 + offset, l + w/2 + offset, t + h * 3/4 + offset )
        | ZOOM_LESS ->
            g.DrawLine( pen, l + w/4 + offset, t + h/2 + offset, l + w * 3/4 + offset, t + h/2 + offset )
        | ROTATE_CLOCKWISE ->
            g.DrawEllipse( pen, l + w/4 + offset, t + h/4 + offset, w/2, h/2 )
            g.FillPolygon( Brushes.Black, [| Point( l + w/2 + offset, t + h * 3/4 + offset );
                                             Point( l + w/2 + 5 + offset, t + h * 3/4 - 8 + offset );
                                             Point( l + w/2 + 7 + offset, t + h * 3/4 + 2 + offset ) |] )
            use p = new Pen( back, 3.f )
            g.DrawCurve( p, [| Point( l + w / 2 - 1 + offset, t + h/2 + offset ); Point( l + w/2 - 1 + offset, t + h * 4/5 + offset ) |] )
        | ROTATE_COUNTERCLOCKWISE ->
            g.DrawEllipse( pen, l + w / 4 + offset, t + h/4 + offset, w/2, h/2 )
            g.FillPolygon( Brushes.Black, [| Point( l + w/2 + offset, t + h * 3/4 + offset );
                                             Point( l + w/2 - 5 + offset, t + h * 3/4 - 8 + offset );
                                             Point( l + w/2 - 7 + offset, t + h * 3/4 + 2 + offset ) |] )

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


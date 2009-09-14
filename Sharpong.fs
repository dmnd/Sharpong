open System
open System.Drawing
open System.Windows.Forms

let form = new Form(Text="Sharpong", BackColor=Color.Black, ClientSize=new Size(400,400))

let pixelSize = 10
let paddleSize = new Size(pixelSize, 5*pixelSize)
let padding = 15
let mutable paddleL = new Rectangle(new Point(padding, 
                                              form.ClientSize.Height/2 - paddleSize.Height/2), paddleSize)
let mutable paddleR = new Rectangle(new Point(form.ClientSize.Width - padding - paddleSize.Width, 
                                              paddleL.Location.Y), paddleSize)

let ballSize = new Size(pixelSize, pixelSize)
let center = new Point(form.ClientSize.Width/2 - ballSize.Width/2, 
                       form.ClientSize.Height/2 - ballSize.Height/2)
let mutable ball = new Rectangle(center, ballSize)

let mutable playerL = ref 0
let mutable playerR = ref 0

let drawWorld (g:Graphics) = 
    g.FillRectangle(Brushes.White, ball)
    g.FillRectangle(Brushes.White, paddleL)
    g.FillRectangle(Brushes.White, paddleR)
    g.DrawString(playerL.Value.ToString(), new Font("Arial", 12.0f), Brushes.White, new PointF(0.0f, 0.0f))
    g.DrawString(playerR.Value.ToString(), new Font("Arial", 12.0f), Brushes.White, new PointF((float32)(form.ClientSize.Width - padding), 0.0f))
form.Paint.Add(fun e -> drawWorld(e.Graphics))

let random = new Random()
let initialVelocity() = 
    let randDir() = if random.Next(2) = 1 then -1.0f else 1.0f
    new PointF(randDir(), randDir())
    
let mutable v = initialVelocity()

let score player = 
    incr player
    ball.Location <- center
    v <- initialVelocity()

let paddleUp = new Point(0, -3)
let paddleDown = new Point(0, 3)

let keyHandler (e:KeyEventArgs) =
    match e.KeyCode with
    | Keys.A -> paddleL.Location <- paddleL.Location + new Size(paddleUp)
    | Keys.Z -> paddleL.Location <- paddleL.Location + new Size(paddleDown)
    | Keys.Up -> paddleR.Location <- paddleR.Location + new Size(paddleUp)
    | Keys.Down -> paddleR.Location <- paddleR.Location + new Size(paddleDown)
    | _ -> ()
form.KeyDown.Add(fun e -> keyHandler e)

let hBetween (value:int) (paddle:Rectangle) =
    value >= paddle.Left && value <= paddle.Right

let passed (ball:Rectangle) (paddle:Rectangle) =
    hBetween ball.Left paddle || hBetween ball.Right paddle

let edgeHeight (ball:Rectangle) (paddle:Rectangle) = 
    ball.Bottom = paddle.Top || ball.Top = paddle.Bottom
    
let hitTop (ball:Rectangle) (paddle:Rectangle) = 
    edgeHeight ball paddle && passed ball paddle

let gameLoop() =
    if ball.Top <= form.ClientRectangle.Top or ball.Bottom >= form.ClientRectangle.Bottom then
        v.Y <- -v.Y
    if ball.Left <= form.ClientRectangle.Left then score playerR
    if ball.Right >= form.ClientRectangle.Right then score playerL
    if hitTop ball paddleL || hitTop ball paddleR then
        v.Y <- -v.Y
    if ball.IntersectsWith(paddleL) || ball.IntersectsWith(paddleR) then
        v.X <- v.X * -1.2f // slowly gets faster
    ball.Location <- ball.Location + new Size(Point.Round(v))
    form.Refresh()
let timer = new Timer(Interval=16)
timer.Tick.Add(fun e -> gameLoop())
timer.Start()

form.Show()
Application.Run(form)
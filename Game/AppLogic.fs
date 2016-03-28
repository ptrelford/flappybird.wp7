﻿// Learn more about F# at http://fsharp.net

namespace Game

open System
open System.Collections.Generic
open System.Linq
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Audio
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.GamerServices
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Input.Touch
open Microsoft.Xna.Framework.Media

/// Bird type
type Bird = { X:float; Y:float; VY:float; IsAlive:bool }

[<AutoOpen>]
module Actions =
    /// Respond to flap command
    let flap (bird:Bird) = { bird with VY = - System.Math.PI }
    /// Applies gravity to bird
    let gravity (bird:Bird) = { bird with VY = bird.VY + 0.1 }
    /// Applies physics to bird
    let physics (bird:Bird) = { bird with Y = bird.Y + bird.VY }
    /// Updates bird with gravity & physics
    let update = gravity >> physics
 
    /// Generates the level's tube positions
    let generateLevel n =
       let rand = System.Random()
       [for i in 1..n -> 50+(i*150), 32+rand.Next(160)]

type Game() as this =
   inherit Microsoft.Xna.Framework.Game()
   do this.Content.RootDirectory <- "Content"
   do this.Window.Title <- "Flap me"
   let graphics = new GraphicsDeviceManager(this)
   do graphics.PreferredBackBufferWidth <- 288
   do graphics.PreferredBackBufferHeight <- 440
   let mutable spriteBatch : SpriteBatch = null
   let mutable bg : Texture2D = null
   let mutable ground : Texture2D = null
   let mutable tube1 : Texture2D = null
   let mutable tube2 : Texture2D = null
   let mutable bird_sing : Texture2D = null
   let mutable lastKeyState = KeyboardState()
   let mutable lastMouseState = MouseState()
   let level = generateLevel 10
   let mutable flappy = { X = 30.0; Y = 150.0; VY = 0.0; IsAlive=true }
   let flapMe () = if flappy.IsAlive then flappy <- flap flappy
   let mutable scroll = 0

   override this.LoadContent() =
      spriteBatch <- new SpriteBatch(this.GraphicsDevice)
      //let load = loadImage this.GraphicsDevice
      let load name = this.Content.Load<Texture2D>(name)
      bg <- load "bg"
      ground <- load "ground"
      tube1 <- load "tube1"
      tube2 <- load "tube2"
      bird_sing <- load "bird_sing"
   
   override this.Update(gameTime) =
      scroll <- scroll - 1
      let currentKeyState = Keyboard.GetState()
      let currentMouseState = Mouse.GetState()
      let isKeyPressedSinceLastFrame key =
         currentKeyState.IsKeyDown(key) && lastKeyState.IsKeyUp(key)
      let isMouseClicked () =
         currentMouseState.LeftButton = ButtonState.Pressed &&
         lastMouseState.LeftButton = ButtonState.Released
      if isKeyPressedSinceLastFrame Keys.Space || isMouseClicked () 
      then flapMe ()
      flappy <- update flappy
      lastKeyState <- currentKeyState
      lastMouseState <- currentMouseState     
   
   override this.Draw(gameTime) =
      this.GraphicsDevice.Clear Color.White
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)
      let draw (texture:Texture2D) (x,y) =
         spriteBatch.Draw(texture, Rectangle(x,y,texture.Width,texture.Height), Color.White)      
      draw bg (0,0)
      draw bird_sing (int flappy.X,int flappy.Y)
      for (x,y) in level do
         let x = x+scroll        
         draw tube1 (x,-320+y)
         draw tube2 (x,y+100)
      draw ground (0,360)
      spriteBatch.End()

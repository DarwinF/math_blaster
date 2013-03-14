using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Math_Blaster
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MathBlaster : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //initialize the external classes
        Game_GUI Graphics;
        Game_Assets assetClass;
        QuestionCrafter questionClass;

        //textures for the menu
        Texture2D Background;
        Texture2D Rocket;
        Texture2D gameOverScreen;

        //menu items
        SpriteFont menufont;
        SpriteFont subtitlefont;
        SpriteFont creditFont;

        //enum for the screen
        private enum Screen
        {
            Menu = 1,
            Options = 2,
            Instructions = 3,
            Game =4

        };
        Screen currentScreen = Screen.Menu;

        //item locations
        Vector2 startGameItemPosition;
        Vector2 optionsItemPosition;
        Vector2 instructionsItemPosition;
        Vector2 exitItemPosition;
        Vector2 creditItemPosition;
        //instructions menu
        Vector2 instructionsPosition;
        Vector2 titleItemPosition;
        //options menu item positions
        Vector2 OptionsTitlePosition;
        Vector2 DifficultyItemPosition;
        Vector2 EasyItemPosition;
        Vector2 MediumItemPosition;
        Vector2 HardItemPosition;
        Vector2 SpeedTitlePosition;
        Vector2 SlowPosition;
        Vector2 FastPosition;
        Vector2 SaveItemPosition;
        Vector2 optionInstructions;
        
        

        //menu font colors
        Color StartGameColor = Color.Yellow;
        Color InstructionColor = Color.Green;
        Color OptionColor = Color.Green;
        Color ExitColor = Color.Green;
        Color TitleColor = Color.Green;
        Color CreditColor = Color.Green;
        //option font colors
        Color EasyColor = Color.Green;
        Color MediumColor = Color.Green;
        Color HardColor = Color.Green;
        Color SlowColor = Color.LawnGreen;
        Color FastColor = Color.LawnGreen;
        Color SaveColor = Color.LawnGreen;
        

        //enum for menu items
        private enum MenuItem
        {
            StartGame = 1,
            Options = 2,
            Instructons = 3,
            Exit =4
            
        };
        MenuItem currentMenuItem = MenuItem.StartGame;

        private enum Difficulty
        {
            Easy = 1,
            Medium = 2,
            Hard = 3
           
        };
        Difficulty currentDfficultyItem = Difficulty.Easy;

        private enum Speed
        {
            slow =1,
            fast =2
        };
        Speed currentSpeed = Speed.slow;

        bool OptionEnum = true;
        bool OptionSave = false;
        bool SaveColorBool = false;

        private bool timeToDrawQuestions = false;
        private bool answerSelected = false;
        private bool difficultyFound = false;
        private int waitIndex = 0;
        private int waitTimer = 0;

        //pad states
        GamePadState pad1;

        //oldpad
        GamePadState oldpad1;


        

        public MathBlaster()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferHeight = 320;
            graphics.PreferredBackBufferWidth = 240;
            graphics.ApplyChanges();

            //initialize the pad1
            if (GamePad.GetState(PlayerIndex.One).IsConnected == true)
                pad1 = GamePad.GetState(PlayerIndex.One);

            oldpad1 = pad1;

            //initialize the menu(s) item positions
            startGameItemPosition = new Vector2(6,69);
            optionsItemPosition = new Vector2(6,105);
            exitItemPosition = new Vector2(6,174);
            instructionsItemPosition = new Vector2(6,139);
            titleItemPosition = new Vector2(0, 0);
            creditItemPosition = new Vector2(7, 268);
            instructionsPosition = new Vector2(5,55);
            OptionsTitlePosition = new Vector2(0,0);
            DifficultyItemPosition = new Vector2(0,55);
            EasyItemPosition = new Vector2(85, 55);
            MediumItemPosition = new Vector2(85, 75);
            HardItemPosition = new Vector2(85, 95);
            SpeedTitlePosition = new Vector2(0,125);
            SlowPosition = new Vector2(75, 125);
            FastPosition = new Vector2(75,155);
            SaveItemPosition = new Vector2(0, 205);
            optionInstructions = new Vector2(0, 245);

            

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //load the external class items
            Graphics = new Game_GUI();
            Graphics.SetPositionValues();
            Graphics.SetTexture(this.Content);
            Graphics.SetFonts(this.Content);

            assetClass = new Game_Assets();
            assetClass.LoadContent(this.Content);
            assetClass.SetPositionValues();

            questionClass = new QuestionCrafter();
            questionClass.LoadThings(this.Content);


            //load the texture for the menu
            Background = Content.Load<Texture2D>("menubackground");
            Rocket = Content.Load<Texture2D>("Rocket");

            //load the fonts
            menufont = Content.Load<SpriteFont>("MenuFont");
            subtitlefont = Content.Load<SpriteFont>("InstructionFont");
            creditFont = Content.Load<SpriteFont>("CreditFont");

            gameOverScreen = Content.Load<Texture2D>("GameOverScreen");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            //update pad1
            pad1 = GamePad.GetState(PlayerIndex.One);
            if (currentScreen == Screen.Menu)
            {
                OptionEnum = true;
                SaveColor = Color.LawnGreen;
                
            }
            //check to see which screen is showing
            switch ((int)currentScreen)
            {
                case 1:
                    UpdateMenuScreen();
                    break;
                case 2:
                    UpdateOptionsScreen();
                    break;
                case 3:
                    UpdateInstructionsScreen();
                    break;
                case 4:
                    UpdateGameScreen();
                    assetClass.DealWithFuel(gameTime);
                    if (questionClass.incrementFuel == true)
                    {
                        assetClass.bonusReady = true;
                        assetClass.CurrentFuel -= 20;
                        questionClass.incrementFuel = false;
                    }
                    if (assetClass.gameOver == true)
                    {
                        if (waitTimer <= 90)
                        {
                            ++waitTimer;
                        }
                        else
                        {
                            waitTimer = 0;
                            assetClass.gameOver = false;
                            questionClass.ExitGameStuff();
                        }
                    }
                    break;
            }

            // TODO: Add your update logic here

            oldpad1 = pad1;

            base.Update(gameTime);
        }

        private void questionUpdateStuff()
        {
            if (pad1.Buttons.B == ButtonState.Pressed && oldpad1.Buttons.B == ButtonState.Released)
            {
                if (questionClass.firstQuestion == true)
                {
                    questionClass.CraftAQuestion(true);
                }
                else
                {
                    answerSelected = true;
                }
            }

            if (answerSelected == true)
            {
                if (questionClass.firstQuestion == false)
                {
                    if (waitIndex <= 60)
                    {
                        questionClass.SelectAnswers();
                        ++waitIndex;
                    }
                    else
                    {
                        questionClass.CraftAQuestion(true);
                        waitIndex = 0;
                        answerSelected = false;
                    }
                }
            }

            assetClass.CurrentScore = questionClass.Score;
        }

        private void UpdateMenuScreen()
        {
            UpdateMenuDpad();

            UpdateMenuItemColor();

            UpdateBButton();
        }

        private void UpdateOptionsScreen()
        {
            
            if (pad1.Buttons.B == ButtonState.Pressed && oldpad1.Buttons.B == ButtonState.Released && SaveColor == Color.Yellow)
            {
                
                
                    currentScreen = Screen.Menu;
                
            }
            

            UpdateOptionDpad();
           
            UpdateOptionItemColor();
        }

        private void UpdateOptionItemColor()
        {
            switch ((int)currentDfficultyItem)
            {
                case 1:
                    EasyColor = Color.Yellow;
                    MediumColor = Color.LawnGreen;
                    HardColor = Color.LawnGreen;
                    break;
                case 2:
                    EasyColor = Color.LawnGreen;
                    MediumColor = Color.Yellow;
                    HardColor = Color.LawnGreen;
                    break;
                case 3:
                    EasyColor = Color.LawnGreen;
                    MediumColor = Color.LawnGreen;
                    HardColor = Color.Yellow;
                    break;

            }


            switch ((int)currentSpeed)
            {
                case 1:
                    SlowColor = Color.Yellow;
                    FastColor = Color.LawnGreen;
                    break;
                case 2:
                    SlowColor = Color.LawnGreen;
                    FastColor = Color.Yellow;
                    break;
            }
        }

        private void UpdateOptionDpad()
        {
            if (pad1.DPad.Down == ButtonState.Pressed && oldpad1.DPad.Down == ButtonState.Released && OptionEnum == true)
            {
                if (currentDfficultyItem == Difficulty.Hard)
                {
                    currentDfficultyItem = Difficulty.Easy;
                }
                else
                {
                   currentDfficultyItem++;
                }
            }

            if (pad1.DPad.Up == ButtonState.Pressed && oldpad1.DPad.Up == ButtonState.Released && OptionEnum == true)
            {
                if (currentDfficultyItem == Difficulty.Easy)
                {
                    currentDfficultyItem = Difficulty.Hard;
                }
                else
                {
                    currentDfficultyItem--;
                }
            }
            if (pad1.Buttons.B == ButtonState.Pressed && oldpad1.Buttons.B == ButtonState.Released && OptionEnum == true)
            {
                OptionEnum = false;
                OptionSave = true;
            }
            else if (pad1.Buttons.B == ButtonState.Pressed && oldpad1.Buttons.B == ButtonState.Released && OptionEnum == false && OptionSave == true)
            {
                OptionSave = false;
                SaveColorBool = true;
                SaveColor = Color.Yellow;
            }
            

            if (pad1.DPad.Down == ButtonState.Pressed && oldpad1.DPad.Down == ButtonState.Released &&OptionEnum == false && OptionSave== true)
            {
                if (currentSpeed == Speed.fast)
                {
                    currentSpeed = Speed.slow;
                }
                else
                {
                    currentSpeed++;
                }
            }
            
            if (pad1.DPad.Up == ButtonState.Pressed && oldpad1.DPad.Up == ButtonState.Released && OptionEnum == false && OptionSave == true)
            {
                
                if (currentSpeed == Speed.slow)
                {
                    currentSpeed = Speed.fast;
                }
                
                else
                {
                    currentSpeed--;
                }
                
            }

        }

        private void UpdateInstructionsScreen()
        {
            if (pad1.Buttons.Back == ButtonState.Pressed && oldpad1.Buttons.Back == ButtonState.Released)
            {
                currentScreen = Screen.Menu;
            }
        }

        private void UpdateGameScreen()
        {
            if (difficultyFound == false)
            {
                if (currentDfficultyItem == Difficulty.Easy)
                    questionClass.FindDifficulty(1);
                if (currentDfficultyItem == Difficulty.Medium)
                    questionClass.FindDifficulty(2);
                if (currentDfficultyItem == Difficulty.Hard)
                    questionClass.FindDifficulty(3);

                if (currentSpeed == Speed.fast)
                    assetClass.FuelDecrement = 30;
                if (currentSpeed == Speed.slow)
                    assetClass.FuelDecrement = 60;

                difficultyFound = true;
            }

            questionClass.UpdateMethodStuff();

            questionUpdateStuff();            

            if (pad1.Buttons.Back == ButtonState.Pressed && oldpad1.Buttons.Back == ButtonState.Released)
            {
                currentScreen = Screen.Menu;
                difficultyFound = false;
            }
        }

        private void UpdateMenuDpad()
        {
            if (pad1.DPad.Down == ButtonState.Pressed && oldpad1.DPad.Down == ButtonState.Released)
            {
                if (currentMenuItem == MenuItem.Exit)
                {
                    currentMenuItem = MenuItem.StartGame;
                }
                else
                {
                    currentMenuItem++;
                }
            }

            if (pad1.DPad.Up == ButtonState.Pressed && oldpad1.DPad.Up == ButtonState.Released)
            {
                if (currentMenuItem == MenuItem.StartGame)
                {
                    currentMenuItem = MenuItem.Exit;
                }
                else
                {
                    currentMenuItem--;
                }
            }

        }

        private void UpdateMenuItemColor()
        {
            switch ((int)currentMenuItem)
            {
                case 1:
                    StartGameColor = Color.Yellow;
                    InstructionColor = Color.Green;
                    OptionColor = Color.Green;
                    ExitColor = Color.Green;
                    break;
                case 2:
                    StartGameColor = Color.Green;
                    InstructionColor = Color.Green;
                    OptionColor = Color.Yellow;
                    ExitColor = Color.Green;
                    break;
                case 3:
                    StartGameColor = Color.Green;
                    InstructionColor = Color.Yellow;
                    OptionColor = Color.Green;
                    ExitColor = Color.Green;
                    break;
                case 4:
                    StartGameColor = Color.Green;
                    InstructionColor = Color.Green;
                    OptionColor = Color.Green;
                    ExitColor = Color.Yellow;
                    break;
            }
        }
        
        private void UpdateBButton()
        {
            if (pad1.Buttons.B == ButtonState.Pressed && oldpad1.Buttons.B == ButtonState.Released)
            {
                switch (currentMenuItem)
                {
                    case(MenuItem.StartGame):
                        currentScreen = Screen.Game;
                        break;
                    case(MenuItem.Options):
                        currentScreen = Screen.Options;
                        break;
                    case(MenuItem.Instructons):
                        currentScreen = Screen.Instructions;
                        break;
                    case (MenuItem.Exit):
                        this.Exit();
                        break;
                }

            }
        }
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            switch ((int)currentScreen)
            {
                case 1:
                    DrawMenuScreen(spriteBatch);
                    break;
                case 2:
                    DrawOptionsScreen(spriteBatch);
                    break;
                case 3:
                    DrawInstructionsScreen(spriteBatch);
                    break;
                case 4:
                    DrawGameScreen(spriteBatch);
                    break;
            }

            if (questionClass.CurrentState == 2)
                questionClass.DrawQuestionAndAnswers(spriteBatch);

            if (assetClass.gameOver == true)
                spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, 240, 320), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMenuScreen(SpriteBatch spriteBatch)
        {
            //draw the menu background
            spriteBatch.Draw(Background, new Rectangle(-65, -72, 376,400),Color.White);
            //draw menu items
            DrawMenuItems(spriteBatch);
        }
        private void DrawMenuItems(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(menufont, "Math Blaster", titleItemPosition, TitleColor);
            spriteBatch.DrawString(subtitlefont, "Start Game", startGameItemPosition, StartGameColor);
            spriteBatch.DrawString(subtitlefont, "Instructions", instructionsItemPosition, InstructionColor);
            spriteBatch.DrawString(subtitlefont, "Options",optionsItemPosition, OptionColor);
            spriteBatch.DrawString(subtitlefont, "Exit", exitItemPosition, ExitColor);
            spriteBatch.DrawString(creditFont, "Created by: Darwin Froese \n           Dilan Dyck", creditItemPosition, CreditColor);
            spriteBatch.Draw(Rocket, new Rectangle(142,65,98,174), Color.White);
        }

        private void DrawOptionsScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, new Rectangle(-65, -72, 376, 400), Color.White);
            spriteBatch.DrawString(menufont, "Options", titleItemPosition, InstructionColor);
            DrawOptionItems(spriteBatch);
        }

        private void DrawOptionItems(SpriteBatch spriteBatch)
        {
            //draw all of the options
            spriteBatch.DrawString(menufont, "Options", OptionsTitlePosition, Color.LawnGreen);
          spriteBatch.DrawString(subtitlefont, "Difficulty:", DifficultyItemPosition, Color.LawnGreen);
            spriteBatch.DrawString(subtitlefont, "Easy ( + and -)", EasyItemPosition,EasyColor );
            spriteBatch.DrawString(subtitlefont, "Medium (X and /)", MediumItemPosition,MediumColor);
            spriteBatch.DrawString(subtitlefont, "Hard (ALL)", HardItemPosition, HardColor);
            spriteBatch.DrawString(subtitlefont, "Save", SaveItemPosition, SaveColor);
            
            spriteBatch.DrawString(subtitlefont, "Speed:",SpeedTitlePosition ,Color.LawnGreen);
            spriteBatch.DrawString(subtitlefont, "Slow", SlowPosition, SlowColor);
            spriteBatch.DrawString(subtitlefont, "Fast", FastPosition, FastColor);
            spriteBatch.DrawString(subtitlefont, "to select diffuculty highlight the\none you want then click the\nplay/pause button to continue", optionInstructions, Color.LawnGreen);
        }

        private void DrawInstructionsScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, new Rectangle(-65, -72, 376, 400), Color.White);
            spriteBatch.DrawString(menufont, "Instructions",titleItemPosition , Color.LawnGreen);
            spriteBatch.DrawString(subtitlefont, "Goal: To stop the rocket \nfrom falling\n\nHow: Answer the questions\nto get more fuel \n \nTo answer the questions\nuse the center pad to\nhighlight the answer you\nwant then click the\nplay/pause button.\nUse the back button to go\nback to first screen", instructionsPosition, Color.LawnGreen);
        }

        private void DrawGameScreen(SpriteBatch spriteBatch)
        {
            assetClass.DrawFuel(spriteBatch);
            Graphics.DrawGUI(spriteBatch);
            assetClass.PlayerScore(spriteBatch);
            questionClass.DrawTexture(spriteBatch);            
        }
    }
}

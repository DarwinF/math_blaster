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
    class Game_Assets
    {
        /**************************************/
        /**                                  **/
        /**        Game_Assets Class         **/
        /**                                  **/
        /**************************************/
        /**                                  **/
        /** This class will draw the players **/
        /**    current score, the players    **/
        /**   current fuel, and will draw    **/
        /**              the rocket          **/
        /**                                  **/
        /**************************************/
        /**        By: Darwin Froese         **/
        /**************************************/

        /* game data */
        private List<Vector2> positionsList = new List<Vector2>();

        private SpriteFont scoreFont;

        private int currentScore;
        private int currentFuel;

        private Texture2D fuelTexture;

        private int currentTime;
        private int timeBetweenDecrements = 30;

        public bool gameOver = false;
        public bool bonusReady = false;

        /* accessor methods */
        public int CurrentScore
        {
            get { return currentScore; }
            set { currentScore = value; }
        }

        public int CurrentFuel
        {
            get { return currentFuel; }
            set { currentFuel = value; }
        }

        public int FuelDecrement
        {
            get { return timeBetweenDecrements; }
            set { timeBetweenDecrements = value; }
        }

        /* initialize positions */
        public void SetPositionValues()
        {
            positionsList.Add(new Vector2(45, 25));         /* score display position */
            positionsList.Add(new Vector2(192, 123));       /* fuel position (33 is the lowest Y value, 213 is the highest Y value) */
            positionsList.Add(new Vector2(0, 0));           /* rocket start position */
            positionsList.Add(new Vector2(0, 0));           /* current rocket position */
        }

        /* load the class content */
        public void LoadContent(ContentManager theContentManager)
        {
            scoreFont = theContentManager.Load<SpriteFont>("ScoreFont");

            fuelTexture = theContentManager.Load<Texture2D>("FuelBar");
        }

        /* draw the players score */
        public void PlayerScore(SpriteBatch theSpriteBatch)
        {
            string score = currentScore.ToString();

            CheckScoreLength();

            theSpriteBatch.DrawString(scoreFont, score, positionsList[0], Color.White);
        }
        
        /* check the length of the score for positioning purposes */
        private void CheckScoreLength()
        {
            if (currentScore >= 10)
                positionsList[0] = new Vector2(38, 25);
            if (currentScore >= 100)
                positionsList[0] = new Vector2(30, 25);
            if (currentScore >= 1000)
                positionsList[0] = new Vector2(22, 25);
        }
        
        /* draw and move the fuel in the fuel gauge */
        public void DrawFuel(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(fuelTexture, positionsList[1], Color.White);
        }

        /* deal with the fuels movment up and down as it is used and replenished */
        public void DealWithFuel(GameTime theGameTime)
        {
            Vector2 tempPosition;

            currentTime++;

            if (currentTime >= timeBetweenDecrements)
            {
                tempPosition = positionsList[1];
                tempPosition.Y += 2f;
                positionsList[1] = tempPosition;
                currentTime = 0;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                positionsList.Clear();
                SetPositionValues();
            }                

            if (positionsList[1].Y >= 213)
            {
                gameOver = true;
                positionsList[1] = new Vector2(192, 123);
            }
            
            if (bonusReady == true)
            {
                Vector2 answerBonus;

                answerBonus = positionsList[1];
                answerBonus.Y += currentFuel;
                if (answerBonus.Y <= 33)
                    answerBonus.Y = 33;
                positionsList[1] = answerBonus;
                currentFuel = 0;
                bonusReady = false;
            }
            
        }
    }
}

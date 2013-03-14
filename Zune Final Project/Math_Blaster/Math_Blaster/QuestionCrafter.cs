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
    class QuestionCrafter
    {
        /*************************************/
        /**                                 **/
        /**          QuestionClass          **/
        /**                                 **/
        /*************************************/
        /**                                 **/
        /**    Draw the questions and the   **/
        /**      answers on the screen      **/
        /**                                 **/
        /*************************************/
        /**        By: Darwin Froese        **/
        /*************************************/

        /* game data */
        private enum GameplayStates { pressToStart = 1, playing = 2 }
        private enum GameDifficulty { easy = 1, medium = 2, hard = 3 }
        private enum QuestionSelected { A = 1, B = 2, C = 3, D = 4 }
        private GameplayStates gameplayState = GameplayStates.pressToStart;
        private GameDifficulty gameDifficulty = GameDifficulty.easy;
        private QuestionSelected questionSelected = QuestionSelected.D; /* default to D so that the you will not get 10 points at the start */

        private Random randomNumber = new Random((int)DateTime.Now.Ticks);

        private int currentOperator = 1;
        private int correctAnswer;
        private int numberOne;
        private int numberTwo;
        private int currentState = 1;

        private int answerOne;
        private int answerTwo;
        private int answerThree;
        private int answerFour;
        private int spotToUse;
        private int score = 0;

        private bool questionNeeded = true;
        public bool firstQuestion = true;
        public bool firstMovement = true;
        public bool incrementFuel = false;

        private Color answerOneColor = Color.CornflowerBlue;
        private Color answerTwoColor = Color.White;
        private Color answerThreeColor = Color.White;
        private Color answerFourColor = Color.White;


        private Texture2D pressToStartImage;
        private SpriteFont questionFont;
        private SpriteFont answerFont;

        private string question;

        GamePadState currentPadState;
        GamePadState oldPadState;
        
        /* accessor method for the currentState of the game */
        public int CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        /* accessor for the score */
        public int Score
        {
            get { return score; }
        }

        /* display the press to start screen and check the game difficulty */
        public void LoadThings(ContentManager theContentManager)
        {
            pressToStartImage = theContentManager.Load<Texture2D>("PressToStart");
            questionFont = theContentManager.Load<SpriteFont>("QuestionFont");
            answerFont = theContentManager.Load<SpriteFont>("AnswersFont");            
        }

        /* find the difficulty the game is set at */
        public void FindDifficulty(int Difficulty)
        {
            if (Difficulty == 1)
            {
                gameDifficulty = GameDifficulty.easy;
                currentOperator = 1;
            }
            if (Difficulty == 2)
            {
                gameDifficulty = GameDifficulty.medium;
                currentOperator = 3;
            }
            if (Difficulty == 3)
            {
                gameDifficulty = GameDifficulty.hard;
                currentOperator = 1;
            }

            if (currentState == 1)
                gameplayState = GameplayStates.pressToStart;
        }

        /* draw a simple texture that tells the user to press the play/pause button to start */
        public void DrawTexture(SpriteBatch theSpriteBatch)
        {

            if (gameplayState == GameplayStates.pressToStart)
                theSpriteBatch.Draw(pressToStartImage, new Rectangle(0, 0, 240, 320), Color.White);
        }

        /* stuff for in the update method. things like processing which screen we should be showing */        
        public void UpdateMethodStuff()
        {
            currentPadState = GamePad.GetState(PlayerIndex.One);

            if (currentPadState.Buttons.B == ButtonState.Pressed &&
                oldPadState.Buttons.B == ButtonState.Released &&
                gameplayState == GameplayStates.pressToStart)
            {
                gameplayState = GameplayStates.playing;
                CraftAQuestion(true);
                currentState = 2;
            }

            if (currentPadState.Buttons.B == ButtonState.Pressed &&
                oldPadState.Buttons.B == ButtonState.Released &&
                gameplayState == GameplayStates.playing)
            {
                if ((int)questionSelected == spotToUse)
                {
                    score += 10;
                    incrementFuel = true;
                }
                else
                {
                    if (score >= 5)
                        score -= 5;
                }
            }

            /* call the method for the movement of the selected answer */
            AnswerSelectionController();

            ExitGameStuff();

            oldPadState = currentPadState;
        }

        public void ExitGameStuff()
        {
            if (currentPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
            {
                firstQuestion = true;
                firstMovement = true;
                gameplayState = GameplayStates.pressToStart;
                currentState = 1;
                score = 0;
            }
        }

        /* determine which answer is selected before and after the dpad is pressed */
        private void AnswerSelectionController()
        {
            if (firstMovement == true)
            {
                questionSelected = QuestionSelected.A;
                firstMovement = false;
            }
                
            #region DPAD Up
            if (currentPadState.DPad.Up == ButtonState.Pressed &&
                oldPadState.DPad.Up == ButtonState.Released &&
                gameplayState == GameplayStates.playing)
            {
                if (questionSelected == QuestionSelected.A)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.CornflowerBlue;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.C;
                }

                else if (questionSelected == QuestionSelected.B)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.CornflowerBlue;
                    questionSelected = QuestionSelected.D;
                }

                else if (questionSelected == QuestionSelected.C)
                {
                    answerOneColor = Color.CornflowerBlue;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.A;
                }

                else if (questionSelected == QuestionSelected.D)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.CornflowerBlue;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.B;
                }
            }
            #endregion
            #region DPAD Down
            if (currentPadState.DPad.Down == ButtonState.Pressed &&
                oldPadState.DPad.Down == ButtonState.Released &&
                gameplayState == GameplayStates.playing)
            {
                if (questionSelected == QuestionSelected.A)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.CornflowerBlue;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.C;
                }

                else if (questionSelected == QuestionSelected.B)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.CornflowerBlue;
                    questionSelected = QuestionSelected.D;
                }

                else if (questionSelected == QuestionSelected.C)
                {
                    answerOneColor = Color.CornflowerBlue;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.A;
                }

                else if (questionSelected == QuestionSelected.D)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.CornflowerBlue;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.B;
                }
            }
            #endregion
            #region DPAD Left
            if (currentPadState.DPad.Left == ButtonState.Pressed &&
                oldPadState.DPad.Left == ButtonState.Released &&
                gameplayState == GameplayStates.playing)
            {
                if (questionSelected == QuestionSelected.A)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.CornflowerBlue;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.B;
                }

                else if (questionSelected == QuestionSelected.B)
                {
                    answerOneColor = Color.CornflowerBlue;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.A;
                }

                else if (questionSelected == QuestionSelected.C)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.CornflowerBlue;
                    questionSelected = QuestionSelected.D;
                }

                else if (questionSelected == QuestionSelected.D)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.CornflowerBlue;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.C;
                }
            }
            #endregion
            #region DPAD Right
            if (currentPadState.DPad.Right == ButtonState.Pressed &&
                oldPadState.DPad.Right == ButtonState.Released &&
                gameplayState == GameplayStates.playing)
            {
                if (questionSelected == QuestionSelected.A)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.CornflowerBlue;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.B;
                }

                else if (questionSelected == QuestionSelected.B)
                {
                    answerOneColor = Color.CornflowerBlue;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.A;
                }

                else if (questionSelected == QuestionSelected.C)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.CornflowerBlue;
                    questionSelected = QuestionSelected.D;
                }

                else if (questionSelected == QuestionSelected.D)
                {
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.CornflowerBlue;
                    answerFourColor = Color.White;
                    questionSelected = QuestionSelected.C;
                }
            }
            #endregion
        }

        /* select answers and change colors of the appropriate answers */
        public void SelectAnswers()
        {
            switch (spotToUse)
            {
                case 1:
                    answerOneColor = Color.Green;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    break;
                case 2:
                    answerOneColor = Color.White;
                    answerTwoColor = Color.Green;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.White;
                    break;
                case 3:
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.Green;
                    answerFourColor = Color.White;
                    break;
                case 4:
                    answerOneColor = Color.White;
                    answerTwoColor = Color.White;
                    answerThreeColor = Color.White;
                    answerFourColor = Color.Green;
                    break;
            }       
        }

        /* create a question to display on the screen */
        public void CraftAQuestion(bool QuestionNeeded)
        {
            /* return all the colors back to white, except for answer one. this is one will be defaulted as selected */
            questionSelected = QuestionSelected.A;
            answerOneColor = Color.CornflowerBlue;
            answerTwoColor = Color.White;
            answerThreeColor = Color.White;
            answerFourColor = Color.White;

            /* raise the flag if we need a question, lower it if we do not */
            questionNeeded = QuestionNeeded;

            /* check if we need a question and if we are actually playing the game */
            if (gameplayState == GameplayStates.playing && questionNeeded == true)
            {
                numberOne = randomNumber.Next(0, 11);
                numberTwo = randomNumber.Next(0, 11);

                /* cycle through the operators and use the right one */
                if (currentOperator == 1 && gameDifficulty == GameDifficulty.easy ||
                    currentOperator == 1 && gameDifficulty == GameDifficulty.hard)
                {
                    correctAnswer = 0;
                    question = numberOne.ToString() + " + " + numberTwo.ToString();
                    correctAnswer = numberOne + numberTwo;
                    spotToUse = randomNumber.Next(1, 5);
                    FillAnswerSlots(spotToUse);
                    currentOperator++;
                    questionNeeded = false;
                }
                else if (currentOperator == 2 && gameDifficulty == GameDifficulty.easy ||
                    currentOperator == 2 && gameDifficulty == GameDifficulty.hard)
                {
                    do
                    {
                        numberOne = randomNumber.Next(0, 11);
                        numberTwo = randomNumber.Next(0, 11);
                    }
                    while (numberOne < numberTwo);

                    correctAnswer = 0;
                    question = numberOne.ToString() + " - " + numberTwo.ToString();
                    correctAnswer = numberOne - numberTwo;
                    spotToUse = randomNumber.Next(1, 5);
                    FillAnswerSlots(spotToUse);
                    if (gameDifficulty == GameDifficulty.easy)
                        currentOperator = 1;
                    else
                        currentOperator++;
                    questionNeeded = false;
                }                
                else if (currentOperator == 3 && gameDifficulty == GameDifficulty.medium ||
                    currentOperator == 3 && gameDifficulty == GameDifficulty.hard)
                {
                    correctAnswer = 0;
                    question = numberOne.ToString() + " X " + numberTwo.ToString();
                    correctAnswer = numberOne * numberTwo;
                    spotToUse = randomNumber.Next(1, 5);
                    FillAnswerSlots(spotToUse);
                    currentOperator++;
                    questionNeeded = false;
                }
                else if (currentOperator == 4 && gameDifficulty == GameDifficulty.medium ||
                    currentOperator == 4 && gameDifficulty == GameDifficulty.hard)
                {
                    do
                    {
                        numberOne = randomNumber.Next(0, 11);
                        numberTwo = randomNumber.Next(1, 11);
                    }
                    while (numberOne % numberTwo != 0);

                    correctAnswer = 0;
                    question = numberOne.ToString() + " / " + numberTwo.ToString();
                    correctAnswer = numberOne / numberTwo;
                    spotToUse = randomNumber.Next(1, 5);
                    FillAnswerSlots(spotToUse);
                    if (gameDifficulty == GameDifficulty.medium)
                        currentOperator = 3;
                    else
                        currentOperator = 1;
                    questionNeeded = false;
                }                
            }

            /* set the first question flag to false */
            firstQuestion = false;
        }

        /* draw the question and answers on the screen */
        public void DrawQuestionAndAnswers(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.DrawString(questionFont, question, new Vector2(10, 222), Color.Black);
            theSpriteBatch.DrawString(answerFont, "A) " + answerOne, new Vector2(10, 260), answerOneColor);
            theSpriteBatch.DrawString(answerFont, "B) " + answerTwo, new Vector2(100, 260), answerTwoColor);
            theSpriteBatch.DrawString(answerFont, "C) " + answerThree, new Vector2(10, 300), answerThreeColor);
            theSpriteBatch.DrawString(answerFont, "D) " + answerFour, new Vector2(100, 300), answerFourColor);
        }
        
        /* based on the random number chosen, place the correct answer in the corresponding spot */
        /* fill the other 3 spots with random numbers */
        private void FillAnswerSlots(int spotForAnswer)
        {
            if (spotForAnswer == 1)
            {
                /* a simple complex do/while loop that checks if any of the numbers are the same. */
                /* if any are the same, the randoms will be re-randomed */
                do
                {
                    answerOne = correctAnswer;
                    answerTwo = randomNumber.Next(0, 101);
                    answerThree = randomNumber.Next(0, 101);
                    answerFour = randomNumber.Next(0, 101);
                }
                while (answerOne == answerTwo || answerOne == answerThree || answerOne == answerFour ||
                        answerTwo == answerOne || answerTwo == answerThree || answerTwo == answerFour ||
                        answerThree == answerOne || answerThree == answerTwo || answerThree == answerFour ||
                        answerFour == answerOne || answerFour == answerTwo || answerTwo == answerThree);
            }
            else if (spotForAnswer == 2)
            {
                do
                {
                    answerOne = randomNumber.Next(0, 101);
                    answerTwo = correctAnswer;
                    answerThree = randomNumber.Next(0, 101);
                    answerFour = randomNumber.Next(0, 101);
                }
                while (answerOne == answerTwo || answerOne == answerThree || answerOne == answerFour ||
                        answerTwo == answerOne || answerTwo == answerThree || answerTwo == answerFour ||
                        answerThree == answerOne || answerThree == answerTwo || answerThree == answerFour ||
                        answerFour == answerOne || answerFour == answerTwo || answerTwo == answerThree);
            }
            else if (spotForAnswer == 3)
            {
                do
                {
                    answerOne = randomNumber.Next(0, 101);
                    answerTwo = randomNumber.Next(0, 101);
                    answerThree = correctAnswer;
                    answerFour = randomNumber.Next(0, 101);
                }
                while (answerOne == answerTwo || answerOne == answerThree || answerOne == answerFour ||
                        answerTwo == answerOne || answerTwo == answerThree || answerTwo == answerFour ||
                        answerThree == answerOne || answerThree == answerTwo || answerThree == answerFour ||
                        answerFour == answerOne || answerFour == answerTwo || answerTwo == answerThree);
            }
            else if (spotForAnswer == 4)
            {
                do
                {
                    answerOne = randomNumber.Next(0, 101);
                    answerTwo = randomNumber.Next(0, 101);
                    answerThree = randomNumber.Next(0, 101);
                    answerFour = correctAnswer;
                }
                while (answerOne == answerTwo || answerOne == answerThree || answerOne == answerFour ||
                        answerTwo == answerOne || answerTwo == answerThree || answerTwo == answerFour ||
                        answerThree == answerOne || answerThree == answerTwo || answerThree == answerFour ||
                        answerFour == answerOne || answerFour == answerTwo || answerTwo == answerThree);
            }
        }
    }
}

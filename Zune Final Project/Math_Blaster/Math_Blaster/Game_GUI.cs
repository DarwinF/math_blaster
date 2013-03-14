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
    class Game_GUI
    {
        /*************************************/
        /**                                 **/
        /**         Game_GUI Class          **/
        /**                                 **/
        /*************************************/
        /**                                 **/
        /**   Draw the score title, fuel    **/
        /**    title, and game background   **/
        /**                                 **/
        /**   Objects with changing values  **/
        /**     will not be drawn here      **/
        /**                                 **/
        /*************************************/
        /**       By: Darwin Froese         **/
        /*************************************/

        private List<Vector2> positionsList = new List<Vector2>();

        private Texture2D background;

        private SpriteFont titleFonts;

        /* initialize positions */
        public void SetPositionValues()
        {
            positionsList.Add(new Vector2(0,0));            /* Background Position */
            positionsList.Add(new Vector2(10,0));           /* Score Title Position */
            positionsList.Add(new Vector2(190,0));          /* Fuel Title Position */
        }
        
        /* initialize texture */
        public void SetTexture(ContentManager theContentManager)
        {
            background = theContentManager.Load<Texture2D>("GameScreenTemplate");
        }

        /* initialize the fonts */
        public void SetFonts(ContentManager theContentManager)
        {
            titleFonts = theContentManager.Load<SpriteFont>("TitleFonts");
        }

        /* draw the items that need to be drawn */
        public void DrawGUI(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(background, positionsList[0], Color.White);
            theSpriteBatch.DrawString(titleFonts, "Score", positionsList[1], Color.White);
            theSpriteBatch.DrawString(titleFonts, "Fuel", positionsList[2], Color.White);
        }
    }
}

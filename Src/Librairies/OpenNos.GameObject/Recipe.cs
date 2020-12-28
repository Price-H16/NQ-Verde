using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.GameObject
{
    public class Recipe : RecipeDTO
    {
        #region Properties

        public List<RecipeItemDTO> Items { get; set; }

        #endregion

        #region Instantiation

        public Recipe(RecipeDTO input)
        {
            Amount = input.Amount;
            ItemVNum = input.ItemVNum;
            RecipeId = input.RecipeId;
        }

        #endregion
    }
}
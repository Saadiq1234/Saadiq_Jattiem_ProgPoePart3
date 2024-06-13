using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagerApp
{
    public delegate void CalorieNotificationHandler(string recipeName, double totalCalories);

    public class RecipeManager
    {
        public event CalorieNotificationHandler CalorieExceeded;

        public class Recipe
        {
            public string Name { get; set; }
            public List<Ingredient> Ingredients { get; set; }
            public List<string> Steps { get; set; }
            public double TotalCalories { get; set; }

            public Recipe(string name, List<Ingredient> ingredients, List<string> steps)
            {
                Name = name;
                Ingredients = ingredients;
                Steps = steps;
                TotalCalories = ingredients.Sum(i => i.Calories);
            }
        }

        public class Ingredient
        {
            public string Name { get; set; }
            public double Quantity { get; set; }
            public string Unit { get; set; }
            public double Calories { get; set; }
            public string FoodGroup { get; set; }
            public double OriginalQuantity { get; set; }

            public Ingredient(string name, double quantity, string unit, double calories, string foodGroup)
            {
                Name = name;
                Quantity = quantity;
                Unit = unit;
                Calories = calories;
                FoodGroup = foodGroup;
                OriginalQuantity = quantity;
            }
        }

        private List<Recipe> recipes;

        public RecipeManager()
        {
            recipes = new List<Recipe>();
        }

        public void AddRecipe(Recipe recipe)
        {
            recipes.Add(recipe);
            if (recipe.TotalCalories > 300)
            {
                CalorieExceeded?.Invoke(recipe.Name, recipe.TotalCalories);
            }
        }

        public List<Recipe> GetRecipes()
        {
            return recipes;
        }

        public void ClearData()
        {
            recipes.Clear();
        }

        public void DeleteRecipe(string recipeName)
        {
            Recipe recipeToDelete = recipes.FirstOrDefault(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
            if (recipeToDelete != null)
            {
                recipes.Remove(recipeToDelete);
            }
        }

        public double CalculateTotalCalories(List<Ingredient> ingredients)
        {
            return ingredients.Sum(i => i.Calories);
        }

        public void ScaleRecipe(string recipeName, double factor)
        {
            Recipe recipe = GetRecipe(recipeName);
            if (recipe != null)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Quantity *= factor;
                }

                recipe.TotalCalories = CalculateTotalCalories(recipe.Ingredients);

                if (recipe.TotalCalories > 300)
                {
                    CalorieExceeded?.Invoke(recipe.Name, recipe.TotalCalories);
                }
            }
        }

        public void ResetQuantities(string recipeName)
        {
            Recipe recipe = GetRecipe(recipeName);
            if (recipe != null)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Quantity = ingredient.OriginalQuantity;
                }

                recipe.TotalCalories = CalculateTotalCalories(recipe.Ingredients);
            }
        }

        private Recipe GetRecipe(string recipeName)
        {
            return recipes.FirstOrDefault(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
        }
    }
}

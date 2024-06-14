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

            public Recipe(string name, List<Ingredient> ingredients, List<string> steps, RecipeManager manager)
            {
                Name = name;
                Ingredients = ingredients;
                Steps = steps;
                TotalCalories = manager.CalculateTotalCalories(ingredients); // Calculate total calories using RecipeManager instance
            }
        }

        public class Ingredient
        {
            public string Name { get; set; }
            public double Quantity { get; set; }
            public string Unit { get; set; }
            public double Calories { get; set; } // Total calories for the current quantity
            public string FoodGroup { get; set; }
            public double OriginalQuantity { get; set; }
            public double OriginalCalories { get; set; } // Total calories for the original quantity

            public Ingredient(string name, double quantity, string unit, double caloriesPerUnit, string foodGroup)
            {
                Name = name;
                Quantity = quantity;
                Unit = unit;
                FoodGroup = foodGroup;
                OriginalQuantity = quantity;
                Calories = caloriesPerUnit * quantity; // Calculate total calories for the given quantity
                OriginalCalories = caloriesPerUnit * quantity; // Store the original total calories
            }
        }

        private List<Recipe> recipes;

        public RecipeManager()
        {
            recipes = new List<Recipe>();
            LoadRecipes(); // Load recipes from storage upon initialization
        }

        public void AddRecipe(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            recipes.Add(recipe);
            SaveRecipes(); // Save recipes after adding a new recipe

            if (recipe.TotalCalories > 300)
            {
                CalorieExceeded?.Invoke(recipe.Name, recipe.TotalCalories);
            }
        }

        public List<Recipe> GetRecipes()
        {
            return recipes.ToList(); // Return a copy to prevent external modification
        }

        public void ClearData()
        {
            recipes.Clear();
            SaveRecipes(); // Save recipes after clearing data
        }

        public void DeleteRecipe(string recipeName)
        {
            Recipe recipeToRemove = recipes.FirstOrDefault(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
            if (recipeToRemove != null)
            {
                recipes.Remove(recipeToRemove);
                SaveRecipes(); // Save recipes after deleting a recipe
            }
        }

        public void ScaleRecipe(string recipeName, double factor)
        {
            Recipe recipe = GetRecipe(recipeName);

            if (recipe != null)
            {
                // Scale each ingredient's quantity and recalculate calories
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Quantity *= factor;
                    ingredient.Calories = ingredient.OriginalCalories * factor; // Recalculate calories based on scaled quantity
                }

                // Update total calories of the recipe
                recipe.TotalCalories = CalculateTotalCalories(recipe.Ingredients);

                // Check if the scaled total calories exceed the limit
                if (recipe.TotalCalories > 300)
                {
                    CalorieExceeded?.Invoke(recipe.Name, recipe.TotalCalories);
                }

                SaveRecipes(); // Save recipes after scaling
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
                    ingredient.Calories = ingredient.OriginalCalories; // Reset to original calories
                }

                // Recalculate total calories after resetting quantities
                recipe.TotalCalories = CalculateTotalCalories(recipe.Ingredients);

                SaveRecipes(); // Save recipes after resetting quantities
            }
        }

        public double CalculateTotalCalories(List<Ingredient> ingredients)
        {
            return ingredients.Sum(i => i.Calories); // Sum total calories for all ingredients
        }

        public Recipe GetRecipe(string recipeName)
        {
            return recipes.FirstOrDefault(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
        }

        private void SaveRecipes()
        {
            // Example: Serialize 'recipes' to a file or database
            // Implementation depends on your storage mechanism
        }

        private void LoadRecipes()
        {
            // Example: Deserialize 'recipes' from a file or database
            // Implementation depends on your storage mechanism
        }
    }
}

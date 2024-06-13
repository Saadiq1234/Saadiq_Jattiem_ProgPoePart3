﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace RecipeManagerApp
{
    public delegate void CalorieNotificationHandler(string recipeName, double totalCalories);

    public class PlaceholderTextBox : TextBox
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(""));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        static PlaceholderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));
        }
    }

    public class RecipeManager
    {
        public event CalorieNotificationHandler CalorieExceeded;

        public class Recipe
        {
            public string Name { get; set; }
            public List<Ingredient> Ingredients { get; set; }
            public List<string> Steps { get; set; }
            public double TotalCalories { get; set; }
        }

        public class Ingredient
        {
            public string Name { get; set; }
            public double Quantity { get; set; }
            public double OriginalQuantity { get; set; }
            public string Unit { get; set; }
            public int Calories { get; set; }
            public string FoodGroup { get; set; }
        }


        private List<Recipe> recipes;

        public RecipeManager()
        {
            recipes = new List<Recipe>();
        }

        public void AddRecipe(Recipe recipe)
        {
            recipes.Add(recipe);
            CalorieExceeded?.Invoke(recipe.Name, recipe.TotalCalories);
        }

        public List<Recipe> GetRecipes()
        {
            return recipes;
        }

        public Recipe GetRecipe(string recipeName)
        {
            return recipes.Find(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
        }

        public void ClearData()
        {
            recipes.Clear();
        }

        public void DeleteRecipe(string recipeName)
        {
            Recipe recipeToDelete = recipes.Find(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
            if (recipeToDelete != null)
            {
                recipes.Remove(recipeToDelete);
            }
        }

        public double CalculateTotalCalories(Recipe recipe)
        {
            double totalCalories = 0;
            foreach (var ingredient in recipe.Ingredients)
            {
                totalCalories += (ingredient.Calories * ingredient.Quantity);
            }
            return totalCalories;
        }

        private static readonly Dictionary<string, string> UnitConversions = new Dictionary<string, string>
    {
        { "grams", "kilograms" },
        { "milliliters", "liters" },
        { "teaspoons", "tablespoons" },
        { "tablespoons", "cups" },
        { "cups", "liters" },
        // Add other conversions as needed
    };

        private static readonly Dictionary<string, double> ConversionFactors = new Dictionary<string, double>
    {
        { "grams", 1000.0 },
        { "milliliters", 1000.0 },
        { "teaspoons", 3.0 },
        { "tablespoons", 16.0 },
        { "cups", 4.22675 },
        // Add other conversion factors as needed
    };

        public void ScaleRecipe(string recipeName, double factor)
        {
            Recipe recipe = GetRecipe(recipeName);
            if (recipe != null)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Quantity *= factor;
                    ingredient.Unit = ConvertUnit(ingredient.Unit, factor);
                }

                recipe.TotalCalories = CalculateTotalCalories(recipe);
                Console.WriteLine("Recipe scaled successfully.");

                if (recipe.TotalCalories > 300)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Warning: Total calories of the recipe exceed 300 after scaling!");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Recipe not found.");
                Console.WriteLine();
            }
        }

        private string ConvertUnit(string unit, double factor)
        {
            if (UnitConversions.ContainsKey(unit) && factor != 1)
            {
                if (factor > 1 && ConversionFactors.ContainsKey(unit))
                {
                    return UnitConversions[unit];
                }
                else if (factor < 1 && ConversionFactors.ContainsValue(1 / factor))
                {
                    return UnitConversions.FirstOrDefault(x => x.Value == unit).Key;
                }
            }
            return unit;
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

                recipe.TotalCalories = CalculateTotalCalories(recipe);
                Console.WriteLine("Quantities reset to original values.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Recipe not found.");
                Console.WriteLine();
            }
        }

        public void DisplayAllRecipes()
        {
            if (recipes.Count == 0)
            {
                Console.WriteLine("No recipes available.");
                Console.WriteLine();
                return;
            }

            recipes.Sort((x, y) => x.Name.CompareTo(y.Name));

            Console.WriteLine("Available Recipes:");
            foreach (var recipe in recipes)
            {
                Console.WriteLine($"Recipe: {recipe.Name}");
                Console.WriteLine("Ingredients:");
                foreach (var ingredient in recipe.Ingredients)
                {
                    Console.WriteLine($"{ingredient.Name}: {ingredient.Quantity} {ingredient.Unit}");
                }
                Console.WriteLine("Steps:");
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {recipe.Steps[i]}");
                }
                Console.WriteLine($"Total Calories: {recipe.TotalCalories}");
                Console.WriteLine("--------------------------------------------");
            }
            Console.WriteLine();
        }

        public void DisplayRecipe(string recipeName)
        {
            Recipe recipe = GetRecipe(recipeName);
            if (recipe != null)
            {
                Console.WriteLine($"Recipe: {recipe.Name}");
                Console.WriteLine("Ingredients:");
                foreach (var ingredient in recipe.Ingredients)
                {
                    Console.WriteLine($"{ingredient.Name}: {ingredient.Quantity} {ingredient.Unit}");
                }
                Console.WriteLine("Steps:");
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {recipe.Steps[i]}");
                }
                Console.WriteLine($"Total Calories: {recipe.TotalCalories}");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Recipe not found.");
                Console.WriteLine();
            }
        }
    }
}

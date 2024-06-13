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

        public void ScaleRecipe(string recipeName)
        {
            Recipe recipe = GetRecipe(recipeName);
            if (recipe != null)
            {
                Console.Write("Enter the scaling factor (0.5, 2, or 3): ");
                double factor = double.Parse(Console.ReadLine());
                Console.WriteLine();

                if (factor == 0.5 || factor == 2 || factor == 3)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        ingredient.Quantity *= factor;
                    }

                    recipe.TotalCalories = CalculateTotalCalories(recipe);
                    Console.WriteLine("Recipe scaled successfully.");

                    if (recipe.TotalCalories > 300)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Warning: Total calories of the recipe exceed 300 after scaling!");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid scaling factor. Recipe scaling failed.");
                }

                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Recipe not found.");
                Console.WriteLine();
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
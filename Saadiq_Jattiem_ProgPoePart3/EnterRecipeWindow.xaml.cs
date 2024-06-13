using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static RecipeManagerApp.RecipeManager;

namespace RecipeManagerApp
{
    public partial class EnterRecipeWindow : Window
    {
        private readonly RecipeManager recipeManager;
        private readonly List<Ingredient> ingredients = new List<Ingredient>();

        public Recipe NewRecipe { get; private set; }

        public EnterRecipeWindow(RecipeManager manager)
        {
            InitializeComponent();
            recipeManager = manager;
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            string name = IngredientNameTextBox.Text;
            if (double.TryParse(IngredientQuantityTextBox.Text, out double quantity) &&
                int.TryParse(IngredientCaloriesTextBox.Text, out int calories))
            {
                string unit = IngredientUnitTextBox.Text;
                string foodGroup = IngredientFoodGroupTextBox.Text;

                var ingredient = new Ingredient
                {
                    Name = name,
                    Quantity = quantity,
                    OriginalQuantity = quantity,
                    Unit = unit,
                    Calories = calories,
                    FoodGroup = foodGroup
                };

                ingredients.Add(ingredient);
                IngredientsListBox.Items.Add($"{name}: {quantity} {unit}, {calories} Cal, {foodGroup}");
                ClearIngredientFields();
            }
            else
            {
                MessageBox.Show("Please enter valid quantity and calories.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e)
        {
            string step = StepTextBox.Text;
            if (!string.IsNullOrWhiteSpace(step))
            {
                StepsListBox.Items.Add(step); // Add step to the ListBox
                StepTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid step.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RecipeNameTextBox.Text))
            {
                MessageBox.Show("Please enter a recipe name.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Collect steps from the ListBox
            var steps = new List<string>();
            foreach (var item in StepsListBox.Items)
            {
                steps.Add(item as string);
            }

            var newRecipe = new Recipe
            {
                Name = RecipeNameTextBox.Text,
                Ingredients = ingredients,
                Steps = steps,
                TotalCalories = CalculateTotalCalories(ingredients)
            };

            recipeManager.AddRecipe(newRecipe); // Add the new recipe to the RecipeManager
            DialogResult = true;
            Close();
        }

        private double CalculateTotalCalories(List<Ingredient> ingredients)
        {
            double totalCalories = 0;
            foreach (var ingredient in ingredients)
            {
                totalCalories += ingredient.Calories * ingredient.Quantity;
            }
            return totalCalories;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ClearIngredientFields()
        {
            IngredientNameTextBox.Clear();
            IngredientQuantityTextBox.Clear();
            IngredientUnitTextBox.Clear();
            IngredientCaloriesTextBox.Clear();
            IngredientFoodGroupTextBox.Clear();
        }
    }
}

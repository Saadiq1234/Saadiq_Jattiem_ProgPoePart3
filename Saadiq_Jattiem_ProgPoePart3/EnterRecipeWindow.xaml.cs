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

                var ingredient = new RecipeManager.Ingredient
                {
                    Name = name,
                    Quantity = quantity,
                    OriginalQuantity = quantity,
                    Unit = unit,
                    Calories = calories,
                    FoodGroup = foodGroup
                };

                IngredientsListBox.Items.Add(ingredient);
                ClearIngredientFields();
            }
            else
            {
                MessageBox.Show("Please enter valid quantity and calories.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberOfStepsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StepsListBox.Items.Clear(); // Clear previous input fields

            if (int.TryParse(NumberOfStepsTextBox.Text, out int numberOfSteps))
            {
                for (int i = 0; i < numberOfSteps; i++)
                {
                    var stepTextBox = new TextBox
                    {
                        Margin = new Thickness(0, 5, 0, 0),
                        Style = (Style)FindResource("PlaceholderTextBoxStyle"),
                        Tag = $"Step {i + 1}"
                    };
                    StepsListBox.Items.Add(stepTextBox);
                }
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

            var ingredients = new List<RecipeManager.Ingredient>();
            foreach (var item in IngredientsListBox.Items)
            {
                ingredients.Add(item as RecipeManager.Ingredient);
            }

            // Collect steps from the ListBox
            var steps = new List<string>();
            foreach (var item in StepsListBox.Items)
            {
                steps.Add(item as string);
            }

            int specifiedNumberOfSteps;
            if (!int.TryParse(NumberOfStepsTextBox.Text, out specifiedNumberOfSteps))
            {
                MessageBox.Show("Please enter a valid number of steps.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (steps.Count != specifiedNumberOfSteps)
            {
                MessageBox.Show($"Please enter {specifiedNumberOfSteps} steps.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newRecipe = new RecipeManager.Recipe
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

        private double CalculateTotalCalories(List<RecipeManager.Ingredient> ingredients)
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

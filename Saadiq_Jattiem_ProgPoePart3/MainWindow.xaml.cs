using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RecipeManagerApp
{
    public partial class MainWindow : Window
    {
        private RecipeManager recipeManager;

        public MainWindow()
        {
            InitializeComponent();
            recipeManager = new RecipeManager();
            recipeManager.CalorieExceeded += RecipeManager_CalorieExceeded; // Subscribe to calorie exceeded event
            RefreshRecipeComboBox(); // Ensure ComboBox is populated initially
        }

        private void EnterRecipeDetails_Click(object sender, RoutedEventArgs e)
        {
            EnterRecipeWindow enterRecipeWindow = new EnterRecipeWindow(recipeManager);
            if (enterRecipeWindow.ShowDialog() == true)
            {
                var newRecipe = enterRecipeWindow.NewRecipe;
                if (newRecipe != null)
                {
                    recipeManager.AddRecipe(newRecipe);
                    RefreshRecipeComboBox(); // Refresh ComboBox after adding a recipe
                    MessageBox.Show("Recipe added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DisplayAllRecipes_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text = "";  // Clear previous output
            foreach (var recipe in recipeManager.GetRecipes())
            {
                DisplayRecipeDetails(recipe);
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            recipeManager.ClearData();
            RefreshRecipeComboBox();
            OutputTextBlock.Text = "All data cleared.";
        }

        private void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe recipe)
            {
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(recipe);
            }
            else
            {
                OutputTextBlock.Text = "Please select a recipe from the ComboBox.";
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string ingredient = FilterIngredientTextBox.Text.ToLower();
            string foodGroup = FilterFoodGroupTextBox.Text.ToLower();
            double maxCalories;
            bool isMaxCaloriesValid = double.TryParse(FilterMaxCaloriesTextBox.Text, out maxCalories);

            var filteredRecipes = recipeManager.GetRecipes().Where(r =>
                (string.IsNullOrEmpty(ingredient) || r.Ingredients.Exists(i => i.Name.ToLower().Contains(ingredient))) &&
                (string.IsNullOrEmpty(foodGroup) || r.Ingredients.Exists(i => i.FoodGroup.ToLower().Contains(foodGroup))) &&
                (!isMaxCaloriesValid || r.TotalCalories <= maxCalories));

            OutputTextBlock.Text = ""; // Clear previous output
            if (!filteredRecipes.Any())
            {
                OutputTextBlock.Text = "No recipes found.";
            }
            else
            {
                foreach (var recipe in filteredRecipes)
                {
                    DisplayRecipeDetails(recipe);
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Exit the application
        }

        private void DisplayRecipeDetails(RecipeManager.Recipe recipe)
        {
            OutputTextBlock.Text += $"Recipe: {recipe.Name}\n";
            OutputTextBlock.Text += "Ingredients:\n";
            foreach (var ingredient in recipe.Ingredients)
            {
                OutputTextBlock.Text += $"{ingredient.Name}: {ingredient.Quantity} {ingredient.Unit}\n";
            }
            OutputTextBlock.Text += "Steps:\n";
            foreach (var step in recipe.Steps)
            {
                OutputTextBlock.Text += $"{step}\n";
            }
            OutputTextBlock.Text += $"Total Calories: {recipe.TotalCalories}\n";
            OutputTextBlock.Text += "--------------------------------------------\n";
        }

        private void RecipeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe selectedRecipe)
            {
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(selectedRecipe);
            }
        }

        private void RefreshRecipeComboBox()
        {
            RecipeComboBox.ItemsSource = null; // Clear current items source
            RecipeComboBox.ItemsSource = recipeManager.GetRecipes(); // Refresh with updated list
        }

        private void RecipeManager_CalorieExceeded(string recipeName, double totalCalories)
        {
            if (totalCalories > 300)
            {
                MessageBox.Show($"Warning: Total calories of recipe '{recipeName}' exceed 300!", "Calorie Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe selectedRecipe)
            {
                if (double.TryParse(ScaleFactorTextBox.Text, out double factor))
                {
                    if (factor == 0.5 || factor == 2 || factor == 3)
                    {
                        recipeManager.ScaleRecipe(selectedRecipe.Name, factor);
                        OutputTextBlock.Text = ""; // Clear previous output
                        DisplayRecipeDetails(selectedRecipe); // Display updated recipe details
                    }
                    else
                    {
                        OutputTextBlock.Text = "Invalid scaling factor. Please enter 0.5, 2, or 3.";
                    }
                }
                else
                {
                    OutputTextBlock.Text = "Invalid input. Please enter a valid number.";
                }
            }
            else
            {
                OutputTextBlock.Text = "Please select a recipe from the ComboBox.";
            }
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe selectedRecipe)
            {
                recipeManager.ResetQuantities(selectedRecipe.Name);
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(selectedRecipe); // Display updated recipe details
            }
            else
            {
                OutputTextBlock.Text = "Please select a recipe from the ComboBox.";
            }
        }
    }
}

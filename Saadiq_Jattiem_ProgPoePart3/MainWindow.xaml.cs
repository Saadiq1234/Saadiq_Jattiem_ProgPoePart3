using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RecipeManagerApp;
using static RecipeManagerApp.RecipeManager;

namespace RecipeManagerApp
{
    public partial class MainWindow : Window
    {
        private RecipeManager recipeManager;
        private double currentScaleFactor = 1.0;

        public MainWindow()
        {
            InitializeComponent();
            recipeManager = new RecipeManager();
            recipeManager.CalorieExceeded += RecipeManager_CalorieExceeded;
            RefreshRecipeComboBox(); // Initialize the ComboBox with recipes
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
                    MessageBox.Show("Recipe added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            // Always refresh ComboBox after adding a recipe
            RefreshRecipeComboBox();
        }
        private void DisplayAllRecipes_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text = "";  // Clear previous output

            var recipes = recipeManager.GetRecipes().OrderBy(r => r.Name); // Sort recipes by name

            foreach (var recipe in recipes)
            {
                DisplayRecipeDetails(recipe);
                OutputTextBlock.Text += "--------------------------------------------\n";
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            recipeManager.ClearData();
            OutputTextBlock.Text = "All data cleared.";

            // Always refresh ComboBox after clearing data
            RefreshRecipeComboBox();
        }

        private void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is Recipe selectedRecipe)
            {
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(selectedRecipe);
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
                (string.IsNullOrEmpty(ingredient) || r.Ingredients.Any(i => i.Name.ToLower().Contains(ingredient))) &&
                (string.IsNullOrEmpty(foodGroup) || r.Ingredients.Any(i => i.FoodGroup.ToLower().Contains(foodGroup))) &&
                (!isMaxCaloriesValid || r.TotalCalories <= maxCalories)).OrderBy(r => r.Name).ToList(); // Sort filtered recipes by name

            OutputTextBlock.Text = ""; // Clear previous output

            if (filteredRecipes.Any())
            {
                foreach (var recipe in filteredRecipes)
                {
                    DisplayRecipeDetails(recipe);
                    OutputTextBlock.Text += "--------------------------------------------\n";
                }
            }
            else
            {
                OutputTextBlock.Text = "No recipes found.";
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Exit the application
        }

        private void DisplayRecipeDetails(Recipe recipe)
        {
            OutputTextBlock.Text += $"Recipe: {recipe.Name}\n";
            OutputTextBlock.Text += "Ingredients:\n";

            foreach (var ingredient in recipe.Ingredients)
            {
                OutputTextBlock.Text += $"- {ingredient.Name}\n";
                OutputTextBlock.Text += $"  Quantity: {ingredient.Quantity} {ingredient.Unit}\n";
                OutputTextBlock.Text += $"  Calories: {ingredient.Calories} Cal\n";
                OutputTextBlock.Text += $"  Food Group: {ingredient.FoodGroup}\n";
                OutputTextBlock.Text += "\n";
            }

            OutputTextBlock.Text += "Steps:\n";
            foreach (var step in recipe.Steps)
            {
                OutputTextBlock.Text += $"- {step}\n";
            }

            OutputTextBlock.Text += $"Total Calories: {recipe.TotalCalories}\n";
            OutputTextBlock.Text += "--------------------------------------------\n";
        }

        private void RefreshRecipeComboBox()
        {
            RecipeComboBox.ItemsSource = null; // Clear the ItemsSource first (optional)
            var recipes = recipeManager.GetRecipes().OrderBy(r => r.Name).ToList();
            RecipeComboBox.ItemsSource = recipes;
            RecipeComboBox.DisplayMemberPath = "Name"; // Ensure DisplayMemberPath is set correctly
        }

        private void RecipeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is Recipe selectedRecipe)
            {
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(selectedRecipe); // Display selected recipe details
            }
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
            if (RecipeComboBox.SelectedItem is Recipe selectedRecipe)
            {
                var updatedRecipe = recipeManager.GetRecipe(selectedRecipe.Name);

                if (updatedRecipe != null)
                {
                    // Ask for confirmation
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to scale the recipe '{updatedRecipe.Name}'?",
                                                              "Confirm Scale Recipe", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        recipeManager.ScaleRecipe(updatedRecipe.Name, currentScaleFactor);
                        OutputTextBlock.Text = ""; // Clear previous output
                        DisplayRecipeDetails(updatedRecipe); // Display updated recipe details
                    }
                }
                else
                {
                    OutputTextBlock.Text = "Recipe not found.";
                }
            }
            else
            {
                OutputTextBlock.Text = "Please select a recipe from the ComboBox.";
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is Recipe selectedRecipe)
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

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is Recipe selectedRecipe)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{selectedRecipe.Name}' recipe?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    recipeManager.DeleteRecipe(selectedRecipe.Name);
                    OutputTextBlock.Text = $"Recipe '{selectedRecipe.Name}' deleted successfully.";
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe to delete.", "Delete Recipe", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Always refresh ComboBox after deleting a recipe
            RefreshRecipeComboBox();
        }

        private void ScaleFactor05CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            currentScaleFactor = 0.5;
            UpdateScaleFactorText();
            UncheckOtherScaleFactors((CheckBox)sender);
        }

        private void ScaleFactor05CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!ScaleFactor2CheckBox.IsChecked == true && !ScaleFactor3CheckBox.IsChecked == true)
            {
                currentScaleFactor = 1.0;
                UpdateScaleFactorText();
            }
        }

        private void ScaleFactor2CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            currentScaleFactor = 2.0;
            UpdateScaleFactorText();
            UncheckOtherScaleFactors((CheckBox)sender);
        }

        private void ScaleFactor2CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!ScaleFactor05CheckBox.IsChecked == true && !ScaleFactor3CheckBox.IsChecked == true)
            {
                currentScaleFactor = 1.0;
                UpdateScaleFactorText();
            }
        }

        private void ScaleFactor3CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            currentScaleFactor = 3.0;
            UpdateScaleFactorText();
            UncheckOtherScaleFactors((CheckBox)sender);
        }

        private void ScaleFactor3CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!ScaleFactor05CheckBox.IsChecked == true && !ScaleFactor2CheckBox.IsChecked == true)
            {
                currentScaleFactor = 1.0;
                UpdateScaleFactorText();
            }
        }

        private void UpdateScaleFactorText()
        {
            ScaleFactorTextBox.Text = currentScaleFactor.ToString();
        }

        private void UncheckOtherScaleFactors(CheckBox checkedBox)
        {
            if (checkedBox != ScaleFactor05CheckBox)
                ScaleFactor05CheckBox.IsChecked = false;

            if (checkedBox != ScaleFactor2CheckBox)
                ScaleFactor2CheckBox.IsChecked = false;

            if (checkedBox != ScaleFactor3CheckBox)
                ScaleFactor3CheckBox.IsChecked = false;
        }
    }
}

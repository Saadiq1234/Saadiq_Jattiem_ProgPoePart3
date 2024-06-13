using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RecipeManagerApp;

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
                OutputTextBlock.Text += "--------------------------------------------\n";
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            recipeManager.ClearData();
            RefreshRecipeComboBox(); // Clear and refresh the ComboBox
            OutputTextBlock.Text = "All data cleared.";
        }

        private void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe selectedRecipe)
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
                (!isMaxCaloriesValid || r.TotalCalories <= maxCalories)).ToList();

            if (filteredRecipes.Any())
            {
                var filteredRecipesWindow = new FilteredRecipesWindow(filteredRecipes);
                filteredRecipesWindow.Show();
            }
            else
            {
                MessageBox.Show("No recipes found.", "Filter Results", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void RecipeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe selectedRecipe)
            {
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(selectedRecipe); // Display selected recipe details
            }
        }

        private void RefreshRecipeComboBox()
        {
            var recipes = recipeManager.GetRecipes();
            RecipeComboBox.ItemsSource = null; // Clear current items source
            RecipeComboBox.ItemsSource = recipes; // Set ComboBox items to current recipes
            RecipeComboBox.DisplayMemberPath = nameof(RecipeManager.Recipe.Name); // Ensure the ComboBox displays recipe names
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
                // Ask for confirmation
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to scale the recipe '{selectedRecipe.Name}'?",
                                                          "Confirm Scale Recipe", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    recipeManager.ScaleRecipe(selectedRecipe.Name, currentScaleFactor);
                    OutputTextBlock.Text = ""; // Clear previous output
                    DisplayRecipeDetails(selectedRecipe); // Display updated recipe details
                }
                // No action needed if 'No' is selected in the message box
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

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe selectedRecipe)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{selectedRecipe.Name}' recipe?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    recipeManager.DeleteRecipe(selectedRecipe.Name); // Pass recipe name
                    RefreshRecipeComboBox(); // Refresh ComboBox after deleting a recipe
                    OutputTextBlock.Text = $"Recipe '{selectedRecipe.Name}' deleted successfully.";
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe to delete.", "Delete Recipe", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

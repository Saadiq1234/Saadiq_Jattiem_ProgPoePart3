using System.Linq;
using System.Windows;

namespace RecipeManagerApp
{
    public partial class MainWindow : Window
    {
        private RecipeManager recipeManager;

        public MainWindow()
        {
            InitializeComponent();
            recipeManager = new RecipeManager();
            recipeManager.CalorieExceeded += RecipeManager_CalorieExceeded;
            RecipeComboBox.ItemsSource = recipeManager.GetRecipes();
        }

        private void RecipeManager_CalorieExceeded(string recipeName, double totalCalories)
        {
            if (totalCalories > 300)
            {
                MessageBox.Show($"Warning: Total calories of {recipeName} exceed 300!", "Calorie Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
                    RecipeComboBox.ItemsSource = recipeManager.GetRecipes(); // Refresh ComboBox items
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
            RecipeComboBox.ItemsSource = recipeManager.GetRecipes(); // Refresh ComboBox items
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
                OutputTextBlock.Text = "Please select a recipe.";
            }
        }

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeComboBox.SelectedItem is RecipeManager.Recipe recipe)
            {
                recipeManager.DeleteRecipe(recipe.Name);
                RecipeComboBox.ItemsSource = recipeManager.GetRecipes(); // Refresh ComboBox items
                OutputTextBlock.Text = $"Recipe '{recipe.Name}' deleted successfully.";
            }
            else
            {
                OutputTextBlock.Text = "Please select a recipe.";
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string ingredient = FilterIngredientTextBox.Text.ToLower();
            string foodGroup = FilterFoodGroupTextBox.Text.ToLower();
            double maxCalories;
            double.TryParse(FilterMaxCaloriesTextBox.Text, out maxCalories);

            var filteredRecipes = recipeManager.GetRecipes().Where(r =>
                (string.IsNullOrEmpty(ingredient) || r.Ingredients.Any(i => i.Name.ToLower().Contains(ingredient))) &&
                (string.IsNullOrEmpty(foodGroup) || r.Ingredients.Any(i => i.FoodGroup.ToLower().Contains(foodGroup))) &&
                (maxCalories == 0 || r.TotalCalories <= maxCalories)).ToList();

            OutputTextBlock.Text = ""; // Clear previous output
            if (filteredRecipes.Count == 0)
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

        private void RecipeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Handle any additional actions when the selection changes, if needed
        }
    }
}

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
            EnterRecipeWindow enterRecipeWindow = new EnterRecipeWindow(recipeManager); // Pass recipeManager to the constructor
            if (enterRecipeWindow.ShowDialog() == true)
            {
                var newRecipe = enterRecipeWindow.NewRecipe;
                if (newRecipe != null)
                {
                    recipeManager.AddRecipe(newRecipe);
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
            OutputTextBlock.Text = "All data cleared.";
        }

        private void DisplayRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = RecipeNameTextBox.Text;
            var recipe = recipeManager.GetRecipe(recipeName);
            if (recipe != null)
            {
                OutputTextBlock.Text = ""; // Clear previous output
                DisplayRecipeDetails(recipe);
            }
            else
            {
                OutputTextBlock.Text = "Recipe not found.";
            }
        }

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = RecipeNameTextBox.Text;
            recipeManager.DeleteRecipe(recipeName);
            OutputTextBlock.Text = $"Recipe '{recipeName}' deleted successfully.";
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
    }
}

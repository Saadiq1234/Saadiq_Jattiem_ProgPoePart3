using System.Collections.Generic;
using System.Windows;
using static RecipeManagerApp.RecipeManager;

namespace RecipeManagerApp
{
    public partial class FilteredRecipesWindow : Window
    {
        public FilteredRecipesWindow(List<Recipe> filteredRecipes)
        {
            InitializeComponent();

            // Display filtered recipes in the ListBox
            FilteredRecipesListBox.ItemsSource = filteredRecipes;
        }
    }
}

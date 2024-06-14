using System.Windows;

namespace RecipeManagerApp
{
    public partial class FilterWindow : Window
    {
        public FilterWindow()
        {
            InitializeComponent();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            // Gather filter criteria
            string ingredient = FilterIngredientTextBox.Text.ToLower();
            string foodGroup = FilterFoodGroupTextBox.Text.ToLower();
            double maxCalories;
            bool isMaxCaloriesValid = double.TryParse(FilterMaxCaloriesTextBox.Text, out maxCalories);

            // Create a filter result object or simply apply the filter and return the result
            DialogResult = true; // Indicate success
        }
    }
}

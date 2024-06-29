using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoleculeCounter;

public partial class MoleculeCounterView : UserControl
{
    public MoleculeCounterView()
    {
        InitializeComponent();
        DataContext = new MoleculeCounterViewModel();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = MyRegex();
        e.Handled = regex.IsMatch(e.Text);
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex MyRegex();
}

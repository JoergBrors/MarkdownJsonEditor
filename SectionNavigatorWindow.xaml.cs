using System.Windows;

namespace MarkdownJsonEditor
{
    public partial class SectionNavigatorWindow : Window
    {
        public int SelectedSectionIndex { get; private set; } = -1;
        public List<Services.MarkdownSection> Sections { get; private set; }

        public SectionNavigatorWindow(List<Services.MarkdownSection> sections)
        {
            InitializeComponent();
            Sections = sections;
            SectionListBox.ItemsSource = sections.Select((s, i) => $"{i + 1}. {s.Title}").ToList();
            
            if (sections.Any())
            {
                SectionListBox.SelectedIndex = 0;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SectionListBox.SelectedIndex >= 0)
            {
                SelectedSectionIndex = SectionListBox.SelectedIndex;
                DialogResult = true;
                Close();
            }
        }

        private void CombineButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSectionIndex = -2; // Special value for "combine all"
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SectionListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SectionListBox.SelectedIndex >= 0)
            {
                EditButton_Click(sender, e);
            }
        }
    }
}

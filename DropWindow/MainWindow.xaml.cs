using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DropWindow
{
    public partial class MainWindow : Window
    {
        int iSrc;
        public MainWindow()
        {
            InitializeComponent();
            
            comboBox1.ItemsSource = NamedBrush.AllNamedBrushes();
            comboBox1.SelectedValuePath = "Brush";
            comboBox1.SelectedIndex = 0;
            comboBox1.Focus();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rect1.Fill = (Brush)comboBox1.SelectedValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listBox1.SelectedIndex = listBox1.Items.Add(comboBox1.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i == -1 || i == listBox1.Items.Count - 1)
            {
                Console.Beep();
                return;
            }
            var x = listBox1.Items[i];
            listBox1.Items[i] = listBox1.Items[i + 1];
            listBox1.Items[i + 1] = x;
            listBox1.SelectedIndex = i + 1;
        }
        class NamedBrush
        {
            static Dictionary<string, SolidColorBrush> colorNames = new Dictionary<string, SolidColorBrush>(141);
            string name;
            static NamedBrush()
            {
                foreach (System.Reflection.PropertyInfo pi in
                    typeof(Colors).GetProperties())
                    colorNames[pi.Name] = new
                        SolidColorBrush((Color)pi.GetValue(null, null));
            }

            NamedBrush(string n)
            {
                name = n;
            }

            public SolidColorBrush Brush
            {
                get { return colorNames[name]; }
            }

            public string Name
            {
                get { return name; }
            }

            public override string ToString()
            {
                return name;
            }

            public static IEnumerable<NamedBrush> AllNamedBrushes()
            {
                return colorNames.Select(e => new NamedBrush(e.Key));
            }

            public static Brush GetBrush(string name)
            {
                return colorNames.ContainsKey(name ?? "") ?
                    colorNames[name] : null;
            }

        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rect2.Fill = NamedBrush.GetBrush((string)listBox1.SelectedItem);
        }

        private void butDelete_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            int i = listBox1.SelectedIndex;
            if (i == -1)
            {
                Console.Beep();
                return;
            }
            listBox1.Items.RemoveAt(i);
            if (i == listBox1.Items.Count)
                i--;
            listBox1.SelectedIndex = i;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i == -1)
                Button_Click_3(null, null);
            else
            {
                listBox1.Items.Insert(i, comboBox1.Text);
                listBox1.SelectedIndex = i;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i <= 0)
            {
                Console.Beep();
                return;
            }
            var x = listBox1.Items[i];
            listBox1.Items[i] = listBox1.Items[i - 1];
            listBox1.Items[i - 1] = x;
            listBox1.SelectedIndex = i - 1;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                Console.Beep();
                return;
            }
            File.WriteAllLines("LISTBOXES.dat",
            listBox1.Items.Cast<string>());
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("LISTBOXES.dat"))
            {
                Console.Beep();
                return;
            }
            listBox1.Items.Clear();
            foreach (var e1 in File.ReadLines("LISTBOXES.dat"))
                listBox1.Items.Add(e1);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        static bool IsMouseOverTarget(Visual target, Point point)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(point);
        }
        static int IndexFromPoint(ListBox lb,
        Func<IInputElement, Point> getPos)
        {
            for (int i = 0; i < lb.Items.Count; i++)
            {
                var lbi = lb.ItemContainerGenerator.ContainerFromIndex(i)
                as ListBoxItem;
                if (lbi == null) continue;
                if (IsMouseOverTarget(lbi, getPos((IInputElement)lbi)))
                    return i;
            }
            return -1;
        }

        private void comboBox1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                DragDrop.DoDragDrop(comboBox1, comboBox1.Text,
                DragDropEffects.Copy);
        }

        private void butAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                iSrc = IndexFromPoint(listBox1, e.GetPosition);
                if (iSrc != -1)
                    DragDrop.DoDragDrop(listBox1,
                    (string)listBox1.Items[iSrc],
                    DragDropEffects.Move);
            }
        }

        private void butAdd_Drop(object sender, DragEventArgs e)
        {
            string s = e.Data.GetData(typeof(string)) as string;
            int iTrg = IndexFromPoint(listBox1, e.GetPosition);
            if (e.AllowedEffects == DragDropEffects.Move)
                listBox1.Items.RemoveAt(iSrc);
            if (iTrg == -1)
                listBox1.SelectedIndex = listBox1.Items.Add(s);
            else
            {
                listBox1.Items.Insert(iTrg, s);
                listBox1.SelectedIndex = iTrg;
            }
        }

        private void butDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void comboBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }
}

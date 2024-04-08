using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Stratbuy
{
    public partial class Form1 : Form
    {
        private IStrategy currentStrategy;

        public Form1()
        {
            InitializeComponent();
            buttonkitchen.Click += Buttonkitchen_Click;
            buttonbathroom.Click += Buttonbathroom_Click;
            buttonbedroom.Click += Buttonbedroom_Click;
            checkedListBox1.ItemCheck += CheckedListBox1_ItemCheck;
            checkedListBox1.SelectionMode = SelectionMode.One;
            buttoncheck.Click += Buttoncheck_Click;
        }

        private void Buttoncheck_Click(object sender, EventArgs e)
        {
            if (currentStrategy == null)
            {
                MessageBox.Show("Виберіть варіант у checkedListBox1", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (decimal.TryParse(textBox1.Text, out decimal budget))
            {
                string description = currentStrategy.GetDescription(budget);
                string[] lines = description.Split('\n');
                string formattedText = "";
                decimal totalCost = 0;

                foreach (string line in lines)
                {
                    formattedText += $"{line}\r\n";

                    string[] parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        if (decimal.TryParse(parts[2].Split(':')[1].Trim(), out decimal price))
                        {
                            totalCost += price;
                        }
                    }
                }

                decimal remainingBudget = budget - totalCost;
                formattedText += $"Сума: {totalCost}\r\n";
                formattedText += $"Залишок: {remainingBudget}";
                textBox2.Text = formattedText;
            }
            else
            {
                MessageBox.Show("Введіть числове значення в textBox1", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (i != e.Index)
                        checkedListBox1.SetItemChecked(i, false);
                }
            }
            SetStrategy();
        }

        private void SetStrategy()
        {
            switch (checkedListBox1.SelectedIndex)
            {
                case 0:
                    currentStrategy = new CheapStrategy();
                    break;
                case 1:
                    currentStrategy = new PriceQualityStrategy();
                    break;
                case 2:
                    currentStrategy = new ExpensiveStrategy();
                    break;
                default:
                    break;
            }
        }

        private void Buttonbedroom_Click(object sender, EventArgs e)
        {
            UpdateButtons(Bedroom.Basic, Bedroom.Everest, Bedroom.Nordic, Bedroom.Stella, Bedroom.Joss);
        }

        private void Buttonbathroom_Click(object sender, EventArgs e)
        {
            UpdateButtons(Bathroom.Basic, Bathroom.Cersanit, Bathroom.Acrylic, Bathroom.Dionaplus, Bathroom.Erida);
        }

        private void Buttonkitchen_Click(object sender, EventArgs e)
        {
            UpdateButtons(Kitchen.Basic, Kitchen.Viant, Kitchen.Seven, Kitchen.Millenium, Kitchen.Vintage);
        }

        private void UpdateButtons(params object[] items)
        {
            var labels = new Label[] { label1, label2, label3, label4, label5 };

            for (int i = 0; i < items.Length && i < labels.Length; i++)
            {
                var selectedItem = items[i];
                string name = selectedItem.GetType().GetProperty("Name").GetValue(selectedItem).ToString();
                string quality = selectedItem.GetType().GetProperty("Quality").GetValue(selectedItem).ToString();
                string price = selectedItem.GetType().GetProperty("Price").GetValue(selectedItem).ToString();

                labels[i].Text = $"Назва: {name}, Якість: {quality}, Ціна: {price}";
            }
        }

        private void UpdateLabels(object selectedItem)
        {
            if (selectedItem is Kitchen)
            {
                Kitchen kitchen = (Kitchen)selectedItem;
                label1.Text = $"Назва: {kitchen.Name}, Якість: {kitchen.Quality}, Ціна: {kitchen.Price}";
            }
            else if (selectedItem is Bathroom)
            {
                Bathroom bathroom = (Bathroom)selectedItem;
                label2.Text = $"Назва: {bathroom.Name}, Якість: {bathroom.Quality}, Ціна: {bathroom.Price}";
            }
            else if (selectedItem is Bedroom)
            {
                Bedroom bedroom = (Bedroom)selectedItem;
                label3.Text = $"Назва: {bedroom.Name}, Якість: {bedroom.Quality}, Ціна: {bedroom.Price}";
            }
        }

        public interface IStrategy
        {
            string GetDescription(decimal budget);
        }

        public class CheapStrategy : IStrategy
        {
            public string GetDescription(decimal budget)
            {
                return $"Кухня: {Kitchen.Basic.Name}, якість: {Kitchen.Basic.Quality}, ціна: {Kitchen.Basic.Price}\n" +
                       $"Ванна: {Bathroom.Basic.Name}, якість: {Bathroom.Basic.Quality}, ціна: {Bathroom.Basic.Price}\n" +
                       $"Спальня: {Bedroom.Basic.Name}, якість: {Bedroom.Basic.Quality}, ціна: {Bedroom.Basic.Price}";
            }
        }

        public class PriceQualityStrategy : IStrategy
        {
            public string GetDescription(decimal budget)
            {
                decimal bedroomBudget = budget / 3;
                decimal kitchenBudget = bedroomBudget;
                decimal bathroomBudget = budget - (bedroomBudget + kitchenBudget);
                var availableBathrooms = new List<Bathroom> { Bathroom.Basic, Bathroom.Cersanit, Bathroom.Acrylic, Bathroom.Dionaplus, Bathroom.Erida };
                Bathroom selectedBathroom = null;

                foreach (var bathroom in availableBathrooms)
                {
                    if (bathroom.Price <= bathroomBudget)
                    {
                        selectedBathroom = bathroom;
                    }
                }

                var availableKitchens = new List<Kitchen> { Kitchen.Basic, Kitchen.Viant, Kitchen.Seven, Kitchen.Millenium, Kitchen.Vintage };
                Kitchen selectedKitchen = null;

                foreach (var kitchen in availableKitchens)
                {
                    if (kitchen.Price <= kitchenBudget)
                    {
                        selectedKitchen = kitchen;
                    }
                }

                var availableBedrooms = new List<Bedroom> { Bedroom.Basic, Bedroom.Everest, Bedroom.Nordic, Bedroom.Stella, Bedroom.Joss };
                Bedroom selectedBedroom = null;

                foreach (var bedroom in availableBedrooms)
                {
                    if (bedroom.Price <= bedroomBudget)
                    {
                        selectedBedroom = bedroom;
                    }
                }

                string description = $"Спальня: {selectedBedroom.Name}, якість: {selectedBedroom.Quality}, ціна: {selectedBedroom.Price}\n" +
                                     $"Кухня: {selectedKitchen.Name}, якість: {selectedKitchen.Quality}, ціна: {selectedKitchen.Price}\n" +
                                     $"Ванна: {selectedBathroom.Name}, якість: {selectedBathroom.Quality}, ціна: {selectedBathroom.Price}";

                return description;
            }
        }

        public class ExpensiveStrategy : IStrategy
        {
            public string GetDescription(decimal budget)
            {
                decimal bedroomBudget = budget / 3;
                decimal kitchenBudget = bedroomBudget;
                decimal bathroomBudget = budget - (bedroomBudget + kitchenBudget);

                var availableBathrooms = new List<Bathroom> { Bathroom.Erida, Bathroom.Dionaplus, Bathroom.Acrylic, Bathroom.Cersanit, Bathroom.Basic };
                Bathroom selectedBathroom = null;

                foreach (var bathroom in availableBathrooms)
                {
                    if (bathroom.Price <= bathroomBudget)
                    {
                        selectedBathroom = bathroom;
                        break;
                    }
                }

                var availableKitchens = new List<Kitchen> { Kitchen.Vintage, Kitchen.Millenium, Kitchen.Seven, Kitchen.Viant, Kitchen.Basic };
                Kitchen selectedKitchen = null;

                foreach (var kitchen in availableKitchens)
                {
                    if (kitchen.Price <= kitchenBudget)
                    {
                        selectedKitchen = kitchen;
                        break;
                    }
                }

                var selectedBedroom = Bedroom.Stella;
                string description = $"Спальня: {selectedBedroom.Name}, якість: {selectedBedroom.Quality}, ціна: {selectedBedroom.Price}\n" +
                                     $"Кухня: {selectedKitchen.Name}, якість: {selectedKitchen.Quality}, ціна: {selectedKitchen.Price}\n" +
                                     $"Ванна: {selectedBathroom.Name}, якість: {selectedBathroom.Quality}, ціна: {selectedBathroom.Price}";

                return description;
            }
        }

        public class Kitchen
        {
            public string Name { get; private set; }
            public int Quality { get; private set; }
            public decimal Price { get; private set; }

            public Kitchen(string name, int quality, decimal price)
            {
                Name = name;
                Quality = quality;
                Price = price;
            }

            public static Kitchen Basic = new Kitchen("Basic", 3, 5000);
            public static Kitchen Viant = new Kitchen("Viant", 3, 12000);
            public static Kitchen Seven = new Kitchen("Seven", 4, 12000);
            public static Kitchen Millenium = new Kitchen("Millenium", 5, 70000);
            public static Kitchen Vintage = new Kitchen("Vintage", 5, 20000);
        }

        public class Bathroom
        {
            public string Name { get; private set; }
            public int Quality { get; private set; }
            public decimal Price { get; private set; }

            public Bathroom(string name, int quality, decimal price)
            {
                Name = name;
                Quality = quality;
                Price = price;
            }

            public static Bathroom Basic = new Bathroom("Basic", 3, 8000);
            public static Bathroom Cersanit = new Bathroom("Cersanit", 4, 15000);
            public static Bathroom Acrylic = new Bathroom("Acrylic", 4, 60000);
            public static Bathroom Dionaplus = new Bathroom("Dionaplus", 5, 75000);
            public static Bathroom Erida = new Bathroom("Erida", 5, 85000);
        }

        public class Bedroom
        {
            public string Name { get; private set; }
            public int Quality { get; private set; }
            public decimal Price { get; private set; }

            public Bedroom(string name, int quality, decimal price)
            {
                Name = name;
                Quality = quality;
                Price = price;
            }

            public static Bedroom Basic = new Bedroom("Basic", 3, 10000);
            public static Bedroom Everest = new Bedroom("Everest", 4, 15000);
            public static Bedroom Nordic = new Bedroom("Nordic", 3, 12000);
            public static Bedroom Stella = new Bedroom("Stella", 5, 50000);
            public static Bedroom Joss = new Bedroom("Joss", 5, 80000);
        }
    }
}

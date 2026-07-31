using Correctum.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Correctum.UI.Controls
{
    public partial class DateRangePicker : StackPanel, INotifyPropertyChanged
    {
        public DateRangePicker()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DateRangeProperty =
            DependencyProperty.Register(
                nameof(DateRange),
                typeof(DateRange),
                typeof(DateRangePicker),
                new FrameworkPropertyMetadata(new DateRange()));

        public DateRange DateRange => (DateRange)GetValue(DateRangeProperty);

        #region UserControl Properties

        public string DisplayValue => DateRange.Count switch
        {
            <= 0 => "",
            1 => $"{DateRange.BeginDate:dd.MM.yyyy}",
            > 1 => $"{DateRange.BeginDate:dd.MM.yyyy} - {DateRange.EndDate:dd.MM.yyyy}",
        };

        public bool IsDropDownOpen
        {
            get;
            set
            {
                if (value == field) return;
                field = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(RotateOpenedIndicator));
            }
        }

        public int RotateOpenedIndicator => IsDropDownOpen ? 180 : 0;

        #endregion

        #region UI Events

        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e) 
        {
            if (sender is not Calendar calendar) return;
            DateRange.SetRangeUnsafe(
                DateOnly.FromDateTime(calendar.SelectedDates.Min()),
                DateOnly.FromDateTime(calendar.SelectedDates.Max()));
            RaisePropertyChanged(nameof(DisplayValue));
            IsDropDownOpen = false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            #if DEBUG
            Debug.Print($"RaisePropertyChanged for {propertyName}");
            #endif

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}

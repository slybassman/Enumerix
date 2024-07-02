using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;

namespace MoleculeCounter;


/// <summary>
/// ViewModel Class
/// </summary>
public class MoleculeCounterViewModel : INotifyPropertyChanged, IDataErrorInfo
{
    private bool _isCounterStatus;
    private string _moleculeCountDisplay = "0";
    private int _moleculeCount;
    private TimeSpan _elapsedTime;
    private readonly DispatcherTimer _dispatcherTimer = new(DispatcherPriority.Render);
    private DateTime _startTime;
    private bool _isTimerActive;
    private bool _isSaturated;
    private const int MaxMoleculeCount = 100;

    public MoleculeCounterViewModel()
    {
        _dispatcherTimer.Tick += TimerTick;
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
    }

    public ICommand StartCounterCommand => new RelayCommand(() =>
    {
        // If timer not currently running then start
        if (_dispatcherTimer.IsEnabled == false)
        {
            _moleculeCount = int.Parse(MoleculeCountDisplay);
            
            // User may be restarting timer when count is at maximum
            if (_moleculeCount >= MaxMoleculeCount)
            {
                IsSaturated = true;
            }
            else
            {
                ElapsedTime = TimeSpan.Zero;

                _dispatcherTimer.Start();
                _startTime = DateTime.Now;

                IsTimerActive = true;
                IsSaturated = false;
            }
        }

        IsCounterStatus = true;
    });

    public ICommand ReturnHomeCommand => new RelayCommand(() =>
    {
        IsCounterStatus = false;
    });

    public ICommand StopCounterCommand => new RelayCommand(() =>
    {
        _dispatcherTimer.Stop();
        IsTimerActive = false;
    });

    public bool IsCounterStatus
    {
        get => _isCounterStatus;
        set => SetField(ref _isCounterStatus, value);
    }

    public bool IsTimerActive 
    { 
        get => _isTimerActive; 
        set => SetField(ref _isTimerActive, value); 
    }

    public bool IsSaturated
    {
        get => _isSaturated;
        set => SetField(ref _isSaturated, value);
    }

    public string? ExperimentName { get; set; }

    public string? SampleName { get; set; }

    public string MoleculeCountDisplay 
    { 
        get => _moleculeCountDisplay; 
        set => SetField(ref _moleculeCountDisplay, value); 
    }

    public TimeSpan ElapsedTime 
    { 
        get => _elapsedTime;
        private set => SetField(ref _elapsedTime, value); 
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;

        field = value;
        
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerTick(object? sender, EventArgs e)
    {
        var elapsedTime = DateTime.Now - _startTime;
        var elapsedSeconds = (int)Math.Floor(elapsedTime.TotalSeconds);
        
        var elapsedLastSeconds = (int)Math.Floor(_elapsedTime.TotalSeconds);
        if (elapsedLastSeconds != elapsedSeconds && IsPrime(elapsedSeconds))
        {
            _moleculeCount++;
            if (_moleculeCount >= MaxMoleculeCount)
            {
                _dispatcherTimer.Stop();
                IsTimerActive = false;
                IsSaturated = true;
            }

            MoleculeCountDisplay = _moleculeCount.ToString();
        }

        ElapsedTime = elapsedTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private static bool IsPrime(int number)
    {
        if (number <= 1)
            return false;

        // check divisibility from 2 up to the square root of the number. If any number divides evenly (remainder is 0)
        for (var x = 2; x <= Math.Sqrt(number); x++)
        {
            if (number % x == 0)
                return false;
        }

        return true;
    }

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            var error = string.Empty;
            switch (columnName)
            {
                case nameof(ExperimentName):
                    if (string.IsNullOrEmpty(ExperimentName))
                    {
                        error = "Please enter some text";
                    }
                    break;
                case nameof(SampleName):
                    if (string.IsNullOrEmpty(SampleName))
                    {
                        error = "Please enter some text";
                    }
                    break;
                case nameof(MoleculeCountDisplay):
                    if (string.IsNullOrEmpty(MoleculeCountDisplay))
                    {
                        error = "Please enter a number";
                    }
                    else if (int.Parse(MoleculeCountDisplay) is <= 0 or > MaxMoleculeCount)
                    {
                        error = $"Value needs to be > 0 and <= {MaxMoleculeCount}";
                    }
                    break;
            }
            
            return error;
        }
    }
}

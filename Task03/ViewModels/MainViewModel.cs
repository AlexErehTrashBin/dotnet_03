using System.Text;
using ReactiveUI;
using Task03.IO;

namespace Task03.ViewModels;

using Models;

public class MainViewModel : ViewModelBase
{
    private readonly ThroughputStream _throughputStream = new();

    private Farm Farm { get; }


    private readonly StringBuilder _readArr = new();
    public string FarmText => _readArr.ToString();

    public void DeleteFarmText()
    {
        _readArr.Clear();
        this.RaisePropertyChanged(nameof(FarmText));
    }

    public MainViewModel()
    {
        
        Farm = new Farm(_throughputStream.OutputStream, (_, _) =>
        {
            var arr = new byte[200];
            try
            {
                _throughputStream.InputStream.ReadExactly(arr);
            }
            catch (EndOfStreamException)
            {
            }
            
            _readArr.Append(Encoding.Default.GetString(arr));
            this.RaisePropertyChanged(nameof(FarmText));
        });
        Thread thread = new(() =>
        {
            Farm.Start();
        });
        thread.Start();
    }
}
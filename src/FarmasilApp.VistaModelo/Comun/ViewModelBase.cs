using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FarmasilApp.VistaModelo.Comun;

/// <summary>
/// Clase base de toda VistaModelo (VL-04). Implementa la notificación de cambios que usa el enlace de datos de WPF.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Avisa a la Vista de que cambió una propiedad. Sin argumento, toma el nombre de quien la llama.</summary>
    protected void Notificar([CallerMemberName] string? propiedad = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
    }

    /// <summary>
    /// Asigna el campo y notifica solo si el valor cambió.
    /// Devuelve <c>true</c> si hubo cambio.
    /// </summary>
    protected bool Establecer<T>(ref T campo, T valor, [CallerMemberName] string? propiedad = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
        {
            return false;
        }

        campo = valor;
        Notificar(propiedad);
        return true;
    }
}

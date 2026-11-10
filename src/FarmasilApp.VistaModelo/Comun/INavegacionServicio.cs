using System.ComponentModel;

namespace FarmasilApp.VistaModelo.Comun;

/// <summary>
/// Servicio de navegación entre pantallas (VP-00): mantiene la VistaModelo que se muestra en la ventana principal.
/// La Vista asocia cada VistaModelo con su pantalla mediante plantillas de datos, así la VistaModelo no conoce ninguna Vista.
/// </summary>
public interface INavegacionServicio : INotifyPropertyChanged
{
    /// <summary>VistaModelo de la pantalla que se muestra ahora, o <c>null</c> si todavía no se navegó a ninguna.</summary>
    ViewModelBase? Actual { get; }

    /// <summary>Muestra la pantalla de <typeparamref name="TViewModel"/>, que debe estar registrada en el contenedor.</summary>
    void NavegarA<TViewModel>() where TViewModel : ViewModelBase;
}

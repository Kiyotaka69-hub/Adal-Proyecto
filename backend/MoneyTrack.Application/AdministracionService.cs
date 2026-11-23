namespace MoneyTrack.Application;

public interface IAdministracionRepository
{
    Task<List<Perfil>> Listar(int pagina, CancellationToken ct);
    Task<bool> CambiarRol(int id, string rol, CancellationToken ct);
}
public sealed class AdministracionService(IAdministracionRepository repo)
{
    public Task<List<Perfil>> Listar(int pagina,CancellationToken ct) => repo.Listar(pagina,ct);
    public async Task CambiarRol(int administrador,int id,string rol,CancellationToken ct)
    {
        if(administrador==id) throw new FinanzasException(409,"Tu propio rol se administra desde otra cuenta.");
        if(!await repo.CambiarRol(id,rol,ct)) throw new FinanzasException(404,"Cuenta no encontrada.");
    }
}

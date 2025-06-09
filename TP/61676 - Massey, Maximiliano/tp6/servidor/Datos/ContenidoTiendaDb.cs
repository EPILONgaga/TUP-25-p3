using Microsoft.EntityFrameworkCore;

public class ContenidoTiendaDb : DbContext
{
    public ContenidoTiendaDb(DbContextOptions<ContenidoTiendaDb> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto {Id= 0, Nombre = "half-life", Descripcion = "el jugador toma el papel de Gordon Freeman, un científico que trabaja en el Centro de Investigación de Black Mesa. Un experimento fallido provoca una caída de resonancia que abre un portal a una dimensión alienígena llamada Xen, invadiendo la instalación. Gordon debe escapar y luchar contra alienígenas y fuerzas gubernamentales (HECU) para sobrevivir y salvar a la humanidad, finalmente enfrentándose al líder alienígena Nihilanth en Xen.", Precio = 5.79, Stock = 10, Imagen = "imagen1.jpg"},
            new Producto {Id= 1, Nombre = "Blasphemous", Descripcion = "Blasphemous es un juego de acción y plataformas sin piedad, con elementos de combate hack-n-slash, ambientado en el retorcido mundo de Cvstodia. Explora, mejora tus habilidades y masacra las hordas de enemigos que se interponen en tu misión para romper el ciclo de condenación eterna.", Precio = 12.49, Stock = 15, Imagen = "imagen2.jpg"},
            new Producto {Id= 2, Nombre = "Hollow Knight", Descripcion = "Hollow Knight es un juego de plataformas y acción en 2D desarrollado por Team Cherry. Ambientado en el reino subterráneo de Hallownest, los jugadores controlan a un caballero sin nombre mientras exploran un vasto mundo lleno de criaturas extrañas, jefes desafiantes y secretos ocultos. El juego destaca por su estilo artístico hermoso, su atmósfera envolvente y su jugabilidad fluida.", Precio = 4.99, Stock = 20, Imagen = "imagen3.jpg"},
            new Producto {Id= 3, Nombre = "Celeste", Descripcion = "Celeste es un juego de plataformas indie desarrollado por Maddy Makes Games. Los jugadores controlan a Madeline mientras escala la montaña Celeste, enfrentándose a desafíos y superando obstáculos. El juego destaca por su jugabilidad precisa, su emotiva narrativa sobre la superación personal y su estilo visual pixel art encantador.", Precio = 9.99, Stock = 25, Imagen = "imagen4.jpg"},
            new Producto {Id= 4, Nombre = "Resident Evli 4", Descripcion = "Resident Evil 4 es un juego de acción y terror desarrollado por Capcom. Los jugadores asumen el papel de Leon S. Kennedy, un agente del gobierno enviado a rescatar a la hija del presidente de los Estados Unidos, Ashley Graham, que ha sido secuestrada por un culto en una aldea rural de España. El juego combina elementos de acción, exploración y resolución de acertijos mientras Leon enfrenta a enemigos infectados por un parásito conocido como Las Plagas.", Precio = 2.99, Stock = 30, Imagen = "imagen5.jpg"},
            new Producto {Id= 5, Nombre = "dark souls", Descripcion = "Dark Souls es un juego de rol de acción desarrollado por FromSoftware. Ambientado en un mundo oscuro y decadente, los jugadores asumen el papel de un no-muerto que busca romper la maldición de la no-muerte. El juego es conocido por su dificultad desafiante, su diseño de niveles intrincado y su narrativa ambiental, donde los jugadores deben enfrentarse a enemigos poderosos y jefes épicos mientras exploran un mundo interconectado.", Precio = 31.99, Stock = 5, Imagen = "imagen6.jpg"}
            );
    }
}
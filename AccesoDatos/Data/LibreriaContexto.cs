using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelosApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Data
{
    public class LibreriaContexto : IdentityDbContext<IdentityUser>
    {
        public LibreriaContexto(DbContextOptions<LibreriaContexto> opciones) : base(opciones)
        {
        }


        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<UsuarioAplicacion> UsuariosAplicacion { get; set; }
        public DbSet<OrdenCompra> OrdenesCompra { get; set; }
        public DbSet<OrdenDetalle> OrdenesDetalle { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { ID = 1, Nombre = "Romance"},
                new Categoria { ID = 2, Nombre = "Terror" },
                new Categoria { ID = 3, Nombre = "Misterio" },
                new Categoria { ID = 4, Nombre = "Fantasía" },
                new Categoria { ID = 5, Nombre = "Biografia" },
                new Categoria { ID = 6, Nombre = "Drama" }
                );

            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    ID = 1,
                    Titulo = "Diarios en el bosque",
                    Autor = "Pablo Lanzelati",
                    Descripcion = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "9783526598435",
                    Precio = 20000,
                    CategoriaID = 6,
                    RutaImagen = @"\imagenes\producto\1.png"
                },
                new Producto
                {
                    ID = 2,
                    Titulo = "Los Velázquez - Memorias de una familia",
                    Autor = "Uma Rouge",
                    Descripcion = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "9782739169302",
                    Precio = 25000,
                    CategoriaID = 6,
                    RutaImagen = @"\imagenes\producto\2.png"
                },
                new Producto
                {
                    ID = 3,
                    Titulo = "Los misterios de Ciudad Perpetua",
                    Autor = "Alan Herrera",
                    Descripcion = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "9781234567897",
                    Precio = 23000,
                    CategoriaID = 3,
                    RutaImagen = @"\imagenes\producto\3.png"
                },
                new Producto
                {
                    ID = 4,
                    Titulo = "Cinco cuentos de familia",
                    Autor = "Pedro Alarcón",
                    Descripcion = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "9781410611772",
                    Precio = 22000,
                    CategoriaID = 2,
                    RutaImagen = @"\imagenes\producto\4.png"
                },
                new Producto
                {
                    ID = 5,
                    Titulo = "El Reino Perdido",
                    Autor = "Claudia Espósito",
                    Descripcion = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "9783161046513",
                    Precio = 24000,
                    CategoriaID = 4,
                    RutaImagen = @"\imagenes\producto\5.png"
                },
                new Producto
                {
                    ID = 6,
                    Titulo = "El destello de tus ojos",
                    Autor = "Gabriela Pérez",
                    Descripcion = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "9782669599750",
                    Precio = 21000,
                    CategoriaID = 1,
                    RutaImagen = @"\imagenes\producto\6.png"
                }
                );
        }
    }
}

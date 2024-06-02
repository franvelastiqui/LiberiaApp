Proyecto donde el usuario podrá comprar libros y el administrador podrá agregar productos y procesar órdenes.

Para poder crear productos y categorías, al crear un usuario, seleccione el rol de administrador.

El cliete podrá:
-Ver todos los libros
-Ver los detalles de un solo libro
-Acceder a un carrito con todos sus productos seleccionados
-Modificar la cantidad de libros dentro del carrito
-Pagar por los libros elegidos
-Tener acceso a sus órdenes de compra y su estado

El empleado podrá hacer todo lo de arriba y:
-Tener acceso las órdenes de compra de todos los clientes
-Actualizar las órdenes de compra según su estado
-Actualizar los datos del cliente al momento de ver una orden de compra del mismo

El administrador podrá hacer todo lo de arriba y:
-Agregar, eliminar y modificar los libros a vender
-Agregar, eliminar y modificar las categorías asignadas de los libros

La aplicación hace uso de la API brindada por Stripe para gestionar los pagos. Para más información, el sitio web docs.stripe.com/api.

Al momento de hace un pago, seleccione la opción de Argentina en https://docs.stripe.com/testing?locale=es-419&testing-method=card-numbers. Este link provee distintas tarjetas de prueba

El proyecto fue creado bajo la plataforma .Net version 8, usando el framework ASP.NET Core.

La aplicación se conecta con SQL Server a través de Entity Framework.

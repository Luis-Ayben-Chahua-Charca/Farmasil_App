namespace Farmasil.Tests
{
    public class UnitTest1
    {
        [Fact]
        //Prueba de ejemplo, no representa ninguna funcionalidad del sistema
        public void Test_SanityCheck_ShouldPass()
        {
            // Prueba básica de verificación para el pipeline de CI
            int resultado = 2 + 2;
            Assert.Equal(4, resultado);
        }
    }
}

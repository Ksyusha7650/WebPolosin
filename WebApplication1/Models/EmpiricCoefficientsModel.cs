namespace WebApplication1.Models;

public record EmpiricCoefficientsModel(
    int IdMaterial,
    int IdEc,
    string Name,
    string? Unit,
    double Value,
    string Symbol);
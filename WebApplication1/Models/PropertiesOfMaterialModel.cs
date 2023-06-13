﻿namespace WebApplication1.Models;

public record PropertiesOfMaterialModel(
    string? Type,
    double Density,
    double SpecificHeat,
    double MeltingPoint);
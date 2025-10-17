using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Abstranctions.Dtos;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.ClientAggregate;

public class BodyMeasurement : Entity
{
    public Guid ClientId { get; private set; }
    public decimal Hip { get; private set; }
    public decimal MaxThigh { get; private set; } //Muslo
    public decimal Biceps { get; private set; }
    public decimal XShoulders { get; private set; } //Contorno de hombros
    public decimal Chest { get; private set; }
    public decimal Height { get; private set; }
    public decimal Weight { get; private set; }

    internal static Result<BodyMeasurement> Create(Guid clientId, BodyMeasurementDto bodyMeasurement)
    {
        var errors = new List<string>();
        bodyMeasurement.Hip.ValidateIsBetween(errors, nameof(bodyMeasurement.Hip), 0);
        bodyMeasurement.MaxThigh.ValidateIsBetween(errors, nameof(bodyMeasurement.MaxThigh), 0);
        bodyMeasurement.Biceps.ValidateIsBetween(errors, nameof(bodyMeasurement.Biceps), 0);
        bodyMeasurement.XShoulders.ValidateIsBetween(errors, nameof(bodyMeasurement.XShoulders), 0);
        bodyMeasurement.Chest.ValidateIsBetween(errors, nameof(bodyMeasurement.Chest), 0);
        bodyMeasurement.Height.ValidateIsBetween(errors, nameof(bodyMeasurement.Height), 0, 300);
        bodyMeasurement.Weight.ValidateIsBetween(errors, nameof(bodyMeasurement.Weight), 0, 500);

        if (errors.Any())
        {
            return Result<BodyMeasurement>.Failure(errors);
        }

        return new BodyMeasurement
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            Hip = bodyMeasurement.Hip,
            MaxThigh = bodyMeasurement.MaxThigh,
            Biceps = bodyMeasurement.Biceps,
            XShoulders = bodyMeasurement.XShoulders,
            Chest = bodyMeasurement.Chest,
            Height = bodyMeasurement.Height,
            Weight = bodyMeasurement.Weight
        };
    }
}

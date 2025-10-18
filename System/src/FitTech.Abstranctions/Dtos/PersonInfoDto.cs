namespace FitTech.Abstranctions.Dtos;

public record PersonInfoDto(string Name, string LastName, string PhoneNumber, DateOnly BirthDate, AddressDto Address);

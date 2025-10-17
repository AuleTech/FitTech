using FitTech.Abstranctions.Dtos.Enums;

namespace FitTech.Abstranctions.Dtos;

public record TrainingSettingsDto(int TotalDaysPerWeek, TrainingGoalEnumDto Goal, Guid[] FavouriteExercises);

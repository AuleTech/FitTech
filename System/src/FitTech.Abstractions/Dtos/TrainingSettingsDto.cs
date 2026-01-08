using FitTech.Abstractions.Dtos.Enums;

namespace FitTech.Abstractions.Dtos;
//TODO: Pending to add FavouriteExercises
public record TrainingSettingsDto(int TotalDaysPerWeek, TrainingGoalEnumDto Goal, Guid[] FavouriteExercises);

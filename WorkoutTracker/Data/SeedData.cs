using WorkoutTracker.Data;
using WorkoutTracker.Models;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        SeedCategories(context);
        SeedWorkoutTypes(context);
        SeedExercises(context);
    }

    private static void SeedCategories(ApplicationDbContext context)
    {
        if (context.Categories.Any())
        {
            return; // Data already seeded
        }

        context.Categories.AddRange(
            new CategoryModel { Name = "Kondition" },
            new CategoryModel { Name = "Bröst" },
            new CategoryModel { Name = "Axlar" },
            new CategoryModel { Name = "Rygg" },
            new CategoryModel { Name = "Bålstyrka" },
            new CategoryModel { Name = "Armar" },
            new CategoryModel { Name = "Ben" },
            new CategoryModel { Name = "Ben Baksida/Rumpa" },
            new CategoryModel { Name = "Rörlighet" },
            new CategoryModel { Name = "Övrigt" }
        );

        context.SaveChanges();
    }

    private static void SeedWorkoutTypes(ApplicationDbContext context)
    {
        if (context.WorkoutTypes.Any())
        {
            return; // Data already seeded
        }

        context.WorkoutTypes.AddRange(
            new WorkoutTypeModel { Name = "Styrketräning" },
            new WorkoutTypeModel { Name = "Konditionsträning" },
            new WorkoutTypeModel { Name = "Funktionell träning" },
            new WorkoutTypeModel { Name = "Rörlighet" },
            new WorkoutTypeModel { Name = "Övrigt" }
        );

        context.SaveChanges();
    }

    private static void SeedExercises(ApplicationDbContext context)
    {
        if (context.Exercises.Any(e => e.UserId == null))
        {
            return; // Data already seeded
        }

        context.Exercises.AddRange(

            // Cardio (1)
            new ExerciseModel { Name = "Intervaller", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Löpning", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Löpband", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Cykling", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Roddmaskin", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Crosstrainer", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Stairmaster", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Promenad", CategoryId = 1, UserId = null },
            new ExerciseModel { Name = "Simning", CategoryId = 1, UserId = null },

            // Brest (2)
            new ExerciseModel { Name = "Bänkpress", CategoryId = 2, UserId = null },
            new ExerciseModel { Name = "Bänkpress lutad", CategoryId = 2, UserId = null },
            new ExerciseModel { Name = "Kabelflyes", CategoryId = 2, UserId = null },
            new ExerciseModel { Name = "Hantelpress", CategoryId = 2, UserId = null },
            new ExerciseModel { Name = "Armhävningar", CategoryId = 2, UserId = null },

            // Shoulders (3)
            new ExerciseModel { Name = "Axelpress", CategoryId = 3, UserId = null },
            new ExerciseModel { Name = "Arnold press", CategoryId = 3, UserId = null },
            new ExerciseModel { Name = "Militärpress", CategoryId = 3, UserId = null },
            new ExerciseModel { Name = "Framåtlyft", CategoryId = 3, UserId = null },
            new ExerciseModel { Name = "Sidolyft hantel", CategoryId = 3, UserId = null },
            new ExerciseModel { Name = "Sidolyft kabel", CategoryId = 3, UserId = null },
            new ExerciseModel { Name = "Shrugs", CategoryId = 3, UserId = null },

            // Back (4)
            new ExerciseModel { Name = "Marklyft", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Raka marklyft", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Latsdrag", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Sittande rodd", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Hantelrodd", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Hyperextensions (rygg)", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Chins", CategoryId = 4, UserId = null },
            new ExerciseModel { Name = "Pull-ups", CategoryId = 4, UserId = null },

            // Core (5)
            new ExerciseModel { Name = "Sit-ups", CategoryId = 5, UserId = null },
            new ExerciseModel { Name = "Plankan", CategoryId = 5, UserId = null },
            new ExerciseModel { Name = "Crunches", CategoryId = 5, UserId = null },
            new ExerciseModel { Name = "Rygglyft", CategoryId = 5, UserId = null },
            new ExerciseModel { Name = "Sidoplanka", CategoryId = 5, UserId = null },
            new ExerciseModel { Name = "Russian twist", CategoryId = 5, UserId = null },
            new ExerciseModel { Name = "Dead bug", CategoryId = 5, UserId = null },

            // Arms (6)
            new ExerciseModel { Name = "Bicepscurl", CategoryId = 6, UserId = null },
            new ExerciseModel { Name = "Hantelcurl", CategoryId = 6, UserId = null },
            new ExerciseModel { Name = "Hammarcurl", CategoryId = 6, UserId = null },
            new ExerciseModel { Name = "Triceps pushdown", CategoryId = 6, UserId = null },
            new ExerciseModel { Name = "Triceps extension", CategoryId = 6, UserId = null },
            new ExerciseModel { Name = "Skull crushers", CategoryId = 6, UserId = null },

            // Legs (7)
            new ExerciseModel { Name = "Benpress", CategoryId = 7, UserId = null },
            new ExerciseModel { Name = "Knäböj", CategoryId = 7, UserId = null },
            new ExerciseModel { Name = "Sumo knäböj", CategoryId = 7, UserId = null },
            new ExerciseModel { Name = "Hack squat", CategoryId = 7, UserId = null },
            new ExerciseModel { Name = "Utfallssteg", CategoryId = 7, UserId = null },
            new ExerciseModel { Name = "Benspark", CategoryId = 7, UserId = null },

            // Glutes/Hamstring (8)
            new ExerciseModel { Name = "Hip Thrust", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Glute bridge", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Liggande lårcurl", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Rumänska marklyft", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Good mornings", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Cable kickbacks", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Hyperextension", CategoryId = 8, UserId = null },
            new ExerciseModel { Name = "Finnen (nordic hamstring curl)", CategoryId = 8, UserId = null },

            // Flexibility (9)
            new ExerciseModel { Name = "Stretching", CategoryId = 9, UserId = null },
            new ExerciseModel { Name = "Yoga", CategoryId = 9, UserId = null },
            new ExerciseModel { Name = "Rörlighetsövningar", CategoryId = 9, UserId = null },

            // Other (10)
            new ExerciseModel { Name = "Uppvärmning", CategoryId = 10, UserId = null },
            new ExerciseModel { Name = "Nedvarvning", CategoryId = 10, UserId = null },
            new ExerciseModel { Name = "Funktionell träning", CategoryId = 10, UserId = null }

        );

        context.SaveChanges();
    }
}
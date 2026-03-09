const allExercises = JSON.parse(document.getElementById('exercises-data')?.textContent || '[]');

window.onload = function () {
    const workoutType = document.getElementById('WorkoutTypeId');
    const removeBtnContainer = document.getElementById('exercise-container');

    // Listen for changes in the workouttype-select and toggle fields
    if (workoutType) {
        workoutType.addEventListener('change', toggleExerciseFields);
        toggleExerciseFields();
    }

    // Eventlistener for the remove buttons, using event delegation on the container
    if (removeBtnContainer) {
        removeBtnContainer.addEventListener('click', removeClick);
        removeBtnContainer.addEventListener('change', handleCategoryChange);
        };

    // Set initial exercise count and control the remove buttons
    exerciseCount = document.querySelectorAll('#exercise-container .exercise-row').length;
    updateRemoveBtns();
};

// Function to handle changes in the category select
function handleCategoryChange(e) {
    if (!e.target.classList.contains('exercise-category')) return;

    // Get the selected category id and the exercise select in the same row
    const categoryId = e.target.value;
    const row = e.target.closest('.exercise-row');
    const exerciseSelect = row.querySelector('.exercise-select');

    exerciseSelect.innerHTML = '<option value="">Välj övning</option>';

    // Filter exercise by category, if any, otherwise show all exercises
    const filtered = categoryId ? allExercises.filter(ex => ex.CategoryId == categoryId) : allExercises;
        
    // Create and append options to the exercise select
    filtered.forEach(ex => {
        const option = document.createElement('option');
        option.value = ex.Id;
        option.textContent = ex.Name;
        exerciseSelect.appendChild(option);
    })
}

// Code for forms, to add exercises to the workout, and to add sets to the exercise
let exerciseCount = 0;

// Function to add a new exercise-row in the form
function addExercise() {
    const container = document.getElementById('exercise-container');

    // Create div for each exercise-row
    const row = document.createElement('div');
    row.classList.add('exercise-row', 'mt-2');

    // Hidden Id input-field
    const idInput = document.createElement('input');
    idInput.type = 'hidden';
    idInput.name = `WorkoutExercises[${exerciseCount}].Id`;
    idInput.value = '0'; // Default value for new exercises
    row.appendChild(idInput);

    // Category select
    const categoryWrapper = document.createElement('div');
    categoryWrapper.classList.add('form-group', 'mt-4');
    const categorySelect = document.createElement('select');
    categorySelect.classList.add('form-control', 'exercise-category');
    const firstCatSelect = document.querySelector('.exercise-category');
    if (firstCatSelect) {
        categorySelect.innerHTML = firstCatSelect.innerHTML;
        categorySelect.value = '';
    }

    categoryWrapper.appendChild(categorySelect);

    /**
     * Exercise select
     */
    const exerciseWrapper = document.createElement('div');
    exerciseWrapper.classList.add('form-group', 'mt-4');
    const exerciseSelect = document.createElement('select');
    exerciseSelect.name = `WorkoutExercises[${exerciseCount}].ExerciseId`;
    exerciseSelect.classList.add('form-control', 'exercise-select');
    exerciseSelect.innerHTML = '<option value="">Välj övning</option>';

    // Append the select to the wrapper
    exerciseWrapper.appendChild(exerciseSelect);

    /**
     * Strength field with sets, reps and weight with attributes and classes
     */
    const strengthFields = document.createElement('div');
    strengthFields.classList.add('strength-fields');

    // Sets
    const setsDiv = document.createElement('div');
    setsDiv.classList.add('form-group', 'mt-4');
    const setsInput = document.createElement('input');
    setsInput.name = `WorkoutExercises[${exerciseCount}].Sets`;
    setsInput.required = true;
    setsInput.type = 'number';
    setsInput.placeholder = 'Antal set';
    setsInput.classList.add('form-control');

    // Reps
    const repsDiv = document.createElement('div');
    repsDiv.classList.add('form-group', 'mt-4');
    const repsInput = document.createElement('input');
    repsInput.name = `WorkoutExercises[${exerciseCount}].Reps`;
    repsInput.type = 'number';
    repsInput.placeholder = 'Repetitioner';
    repsInput.classList.add('form-control');

    // Weight
    const weightDiv = document.createElement('div');
    weightDiv.classList.add('form-group', 'mt-4');
    const weightInput = document.createElement('input');
    weightInput.name = `WorkoutExercises[${exerciseCount}].Weight`;
    weightInput.placeholder = 'Vikt (kg)';
    weightInput.classList.add('form-control');

    // Append select to div, and inputs to their respective divs
    setsDiv.appendChild(setsInput);
    repsDiv.appendChild(repsInput);
    weightDiv.appendChild(weightInput);

    // Add the strength fields to the wrapper
    strengthFields.appendChild(setsDiv);
    strengthFields.appendChild(repsDiv);
    strengthFields.appendChild(weightDiv);

    // Cardio fields
    const cardioFields = document.createElement('div');
    cardioFields.classList.add('cardio-fields');

    // Distance
    const distanceDiv = document.createElement('div');
    distanceDiv.classList.add('form-group', 'mt-4');
    const distanceInput = document.createElement('input');
    distanceInput.name = `WorkoutExercises[${exerciseCount}].Distance`;
    distanceInput.placeholder = 'Distans (km)';
    distanceInput.classList.add('form-control');

    // Details
    const detailsDiv = document.createElement('div');
    detailsDiv.classList.add('form-group', 'mt-4');
    const detailsInput = document.createElement('input');
    detailsInput.name = `WorkoutExercises[${exerciseCount}].Details`;
    detailsInput.placeholder = 'ex. 30min eller 6x30sek';
    detailsInput.classList.add('form-control');

    // Include the cardio fields in their wrapper
    distanceDiv.appendChild(distanceInput);
    detailsDiv.appendChild(detailsInput);
    cardioFields.appendChild(distanceDiv);
    cardioFields.appendChild(detailsDiv);

    // Remove button
    const removeWrapper = document.createElement('div');
    removeWrapper.classList.add('form-group', 'mt-3', 'd-flex', 'justify-content-end');

    const removeBtn = document.createElement('button');
    removeBtn.type = 'button';
    removeBtn.classList.add('btn', 'btn-danger', 'btn-sm', 'remove-ex-btn');
    removeBtn.textContent = 'Ta bort';

    removeWrapper.appendChild(removeBtn);

    // Add all elements to the row
    row.appendChild(categoryWrapper);
    row.appendChild(exerciseWrapper);
    row.appendChild(strengthFields);
    row.appendChild(cardioFields);
    row.appendChild(removeWrapper);

    // Append row to the container
    container.appendChild(row);

    // Increment the exercise count for the next exercise
    exerciseCount++;

    // Call the toggle-function
    toggleExerciseFields();
    updateRemoveBtns();
}

// Show and hide fields based on the selected workout type
function toggleExerciseFields() {
    // Get the selected workout type
    const workoutType = document.getElementById('WorkoutTypeId');
    const selectedOption = workoutType.value;

    // Get strength and cardio fields
    const strengthFields = document.querySelectorAll('.strength-fields');
    const cardioFields = document.querySelectorAll('.cardio-fields');

    if (!selectedOption) return;

    // Toggle visibility based on the selected workout type
    if (selectedOption === '1') {
        strengthFields.forEach(field => field.style.display = 'block');
        cardioFields.forEach(field => field.style.display = 'none');
    } else if (selectedOption === '2' || selectedOption === '3' || selectedOption === '5') {
        strengthFields.forEach(field => field.style.display = 'none');
        cardioFields.forEach(field => field.style.display = 'block');
    } else {
        strengthFields.forEach(field => field.style.display = 'block');
        cardioFields.forEach(field => field.style.display = 'block');
    }
}

// Reindex form fields after an exercise has been removed
function reindexExercises() {
    const rows = document.querySelectorAll('#exercise-container .exercise-row');

    // Loop through each exercise-row and update the name attributes of the input fields
    rows.forEach((row, index) => {
        row.querySelectorAll('input, select').forEach(input => {
            if (!input.name) return;

            // Skip inputs that should not be reindexed
            if (input.name === '__Invariant') return;
            if (input.name === 'WorkoutExercises.Index') return;
            if (/^WorkoutExercises\[\d+\]\.Id$/.test(input.name)) return;
            if (input.classList.contains('exercise-category')) return;

            // Update index
            input.name = input.name.replace(/\[\d+\]/, `[${index}]`);
        });
    });

    // Sync exerciseCount for new exercises to be added after removal, and control the remove buttons
    exerciseCount = rows.length;
    updateRemoveBtns();
};

// Event delegation for remove buttons
function removeClick(e) {
    e.preventDefault();

    if (e.target.classList.contains('remove-ex-btn')) {
        removeExercise(e.target);
    }
}

// Removes the exercise-row of the clicked remove button
function removeExercise(button) {
    const row = button.closest('.exercise-row');

    if (row) {
        row.remove();

        // Reindex the remaining exercises after removal
        reindexExercises();
    }
}


// Hide remove button if there i only one exercise-row
function updateRemoveBtns() {
    const rows = document.querySelectorAll('#exercise-container .exercise-row');
    const removeBtns = document.querySelectorAll('.remove-ex-btn');

    if (rows.length <= 1) {
        removeBtns.forEach(btn => {
            btn.style.display = 'none';
        });
    } else {
        removeBtns.forEach(btn => {
            btn.style.display = 'inline-block';
        });
    }
}

SELECT c.Id as 'TheCohortId', c.Name as 'CohortName',
		s.Id as 'TheStudentId', s.FirstName as 'StudentFirstName', s.LastName as 'StudentLastName', s.SlackHandle as 'StudentSlackHandle',
		i.Id as 'TheInstructorId', i.FirstName as 'InstructorFirstName', i.LastName as 'InstructorLastName', i.SlackHandle as 'InstructorSlackHandle'
FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id LEFT JOIN Instructor i ON i.CohortId = c.Id;

SELECT e.Id as 'TheExerciseId', e.Name as 'ExerciseName', e.Language,
		s.Id as 'TheStudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId
FROM Exercise e LEFT JOIN StudentExercise se ON se.ExerciseId = e.Id
				LEFT JOIN Student s on se.StudentId = s.Id; 


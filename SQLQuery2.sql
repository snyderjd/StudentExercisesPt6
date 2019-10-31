SELECT c.Id as 'TheCohortId', c.Name as 'CohortName',
		s.Id as 'TheStudentId', s.FirstName as 'StudentFirstName', s.LastName as 'StudentLastName', s.SlackHandle as 'StudentSlackHandle',
		i.Id as 'TheInstructorId', i.FirstName as 'InstructorFirstName', i.LastName as 'InstructorLastName', i.SlackHandle as 'InstructorSlackHandle'
FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id LEFT JOIN Instructor i ON i.CohortId = c.Id;


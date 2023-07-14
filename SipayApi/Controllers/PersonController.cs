using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace SipayApi.Controllers{
    public class Person
{
        [DisplayName("Staff person name")]
        public string Name { get; set; }

        [DisplayName("Staff person lastname")]
        public string Lastname { get; set; }

        [DisplayName("Staff person phone number")]
        public string Phone { get; set; }

        [DisplayName("Staff person access level to system")]
        public int AccessLevel { get; set; }

        [DisplayName("Staff person salary")]
        public decimal Salary { get; set; }
}


public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Staff person name is required.")
                .Length(5, 100).WithMessage("Staff person name must be between 5 and 100 characters.");

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage("Staff person lastname is required.")
                .Length(5, 100).WithMessage("Staff person lastname must be between 5 and 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Staff person phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.AccessLevel)
                .InclusiveBetween(1, 5).WithMessage("Staff person access level must be between 1 and 5.");

            RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Staff person salary is required.")
            .GreaterThan(0).WithMessage("Salary must be greater than 0.")
            .InclusiveBetween(5000, 50000).WithMessage("Salary must be between 5000 and 50000.")
            .Must((person, salary) => IsSalaryValid(person.AccessLevel, salary))
            .WithMessage(person =>
            {
                switch (person.AccessLevel)
                {
                    case 1:
                        return "Salary cannot be greater than 10000.";
                    case 2:
                        return "Salary cannot be greater than 20000.";
                    case 3:
                        return "Salary cannot be greater than 30000.";
                    case 4:
                        return "Salary cannot be greater than 40000.";
                    case 5:
                        return "Salary cannot be greater then 50000.";
                    default:
                        return "Access level invalid";
                }
            });
    }

        private bool IsSalaryValid(int accessLevel, decimal salary)
    {
        switch (accessLevel)
        {
            case 1:
                return salary <= 10000;
            case 2:
                return salary <= 20000;
            case 3:
                return salary <= 30000;
            case 4:
                return salary <= 40000;
            case 5:
                return salary <= 50000;
            default:
                return false;
        }
    }
    }

    [ApiController]
    [Route("sipy/api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IValidator<Person> _validator;

        public PersonController()
        {
            _validator = new PersonValidator();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Person person)
        {
            var validationResult = _validator.Validate(person);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            return Ok(person);
        }
    }
}
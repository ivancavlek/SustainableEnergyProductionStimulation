using Acme.Seps.Domain.Base.CommandHandler;
using Acme.Seps.Domain.Parameter.Command;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Acme.Seps.Presentation.Web
{
    [Route("api/[controller]")]
    //[ApiController]
    public class ParameterController : Controller
    {
        private readonly ISepsCommandHandler<CalculateCpiCommand> _calculateCpi;
        private readonly ISepsCommandHandler<CalculateNaturalGasCommand> _calculateNaturalGas;
        private readonly ISepsCommandHandler<CorrectActiveCpiCommand> _correctActiveCpi;
        private readonly ISepsCommandHandler<CorrectActiveNaturalGasCommand> _correctActiveNaturalGas;

        public ParameterController(
            ISepsCommandHandler<CalculateCpiCommand> calculateCpi,
            ISepsCommandHandler<CalculateNaturalGasCommand> calculateNaturalGas,
            ISepsCommandHandler<CorrectActiveCpiCommand> correctActiveCpi,
            ISepsCommandHandler<CorrectActiveNaturalGasCommand> correctActiveNaturalGas)
        {
            _calculateCpi = calculateCpi ?? throw new ArgumentNullException(nameof(calculateCpi));
            _calculateNaturalGas = calculateNaturalGas ?? throw new ArgumentNullException(nameof(calculateNaturalGas));
            _correctActiveCpi = correctActiveCpi ?? throw new ArgumentNullException(nameof(correctActiveCpi));
            _correctActiveNaturalGas = correctActiveNaturalGas ?? throw new ArgumentNullException(nameof(correctActiveNaturalGas));
        }

        // GET: api/<controller>
        [HttpGet]
        [Route("GetActiveCpis")]
        public IEnumerable<string> GetActiveCpis()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("GetActiveNaturalGases")]
        public IEnumerable<string> GetActiveNaturalGases()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("CalculateCpi")]
        public IActionResult CalculateCpi([FromBody]CalculateCpiCommand calculateCpiCommand)
        {
            _calculateCpi.Handle(calculateCpiCommand);
            return Ok(); // not in line with REST pattern, we could return latest value
        }

        [HttpPost]
        [Route("CalculateNaturalGas")]
        public IActionResult CalculateNaturalGas([FromBody]CalculateNaturalGasCommand calculateNaturalGas)
        {
            _calculateNaturalGas.Handle(calculateNaturalGas);
            return Ok(); // not in line with REST pattern, we could return latest value
        }

        [HttpPut("{id}")] // not good, needs correction
        [Route("CorrectActiveCpi")]
        public void Put(int id, [FromBody]CorrectActiveCpiCommand correctActiveCpi)
        {
            _correctActiveCpi.Handle(correctActiveCpi);
        }

        [HttpPut("{id}")] // not good, needs correction
        [Route("CorrectActiveNaturalGas")]
        public void Put(int id, [FromBody]CorrectActiveNaturalGasCommand correctActiveNaturalGas)
        {
            _correctActiveNaturalGas.Handle(correctActiveNaturalGas);
        }
    }
}

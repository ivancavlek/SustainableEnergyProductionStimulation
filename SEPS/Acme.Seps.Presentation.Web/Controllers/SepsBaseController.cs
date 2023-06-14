using Acme.Seps.Presentation.Web.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Acme.Seps.Presentation.Web.Controllers;

public class SepsBaseController : ControllerBase
{
    protected readonly ICqrsMediator _mediator;

    public SepsBaseController(ICqrsMediator mediator) =>
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
}
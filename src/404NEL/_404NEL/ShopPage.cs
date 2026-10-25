using System.Runtime.CompilerServices;
using System.Text;

namespace _404NEL;

public sealed record ShopPage(ShopItem[] Items, int Total)
{
	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200011A.Observe_0020Normalize_0020Evaluate_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		stringBuilder.Append(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200011B.Evaluate_0020Initialize_0020Synchronize_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		if (PrintMembers(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200011C.Observe_0020Worker_0020Finalize_002ENormalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		builder.Append(Items);
		builder.Append(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200011D.Dispatch_0020Dispatch_0020Observe_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		builder.Append(Total.ToString());
		return true;
	}
}

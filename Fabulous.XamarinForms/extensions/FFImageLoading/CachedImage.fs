namespace Fabulous.XamarinForms

open System
open FFImageLoading.Forms
open Fabulous
open FSharp.Data.Adaptive

module ViewAttributes =
    let CachedImageSourceAttribKey = AttributeKey "CachedImage_Source"
    let CachedImageLoadingPlaceholderAttribKey = AttributeKey "CachedImage_LoadingPlaceholder"
    let CachedImageErrorPlaceholderAttribKey = AttributeKey "CachedImage_ErrorPlaceholder"
    let CachedImageCacheTypeAttribKey = AttributeKey "CachedImage_CacheType"
    let CachedImageCacheDurationAttribKey = AttributeKey "CachedImage_CacheDuration"
    let CachedImageCacheKeyFactoryAttribKey = AttributeKey "CachedImage_CacheKeyFactory"
    let CachedImageLoadingDelayAttribKey = AttributeKey "CachedImage_LoadingDelay"
    let CachedImageLoadingPriorityAttribKey = AttributeKey "CachedImage_LoadingPriority"
    let CachedImageCustomDataResolverAttribKey = AttributeKey "CachedImage_CustomDataResolver"
    let CachedImageRetryCountAttribKey = AttributeKey "CachedImage_RetryCount"
    let CachedImageRetryDelayAttribKey = AttributeKey "CachedImage_RetryDelay"
    let CachedImageDownsampleWidthAttribKey = AttributeKey "CachedImage_DownsampleWidth"
    let CachedImageDownsampleHeightAttribKey = AttributeKey "CachedImage_DownsampleHeight"
    let CachedImageDownsampleToViewSizeAttribKey = AttributeKey "CachedImage_DownsampleToViewSize"
    let CachedImageDownsampleUseDipUnitsAttribKey = AttributeKey "CachedImage_DownsampleUseDipUnits"
    let CachedImageFadeAnimationEnabledAttribKey = AttributeKey "CachedImage_FadeAnimationEnabled"
    let CachedImageFadeAnimationDurationAttribKey = AttributeKey "CachedImage_FadeAnimationDuration"
    let CachedImageFadeAnimationForCachedImagesAttribKey = AttributeKey "CachedImage_FadeAnimationForCachedImages"
    let CachedImageBitmapOptimizationsAttribKey = AttributeKey "CachedImage_BitmapOptimizations"
    let CachedImageInvalidateLayoutAfterLoadedAttribKey = AttributeKey "CachedImage_InvalidateLayoutAfterLoaded"
    let CachedImageTransformPlaceholdersAttribKey = AttributeKey "CachedImage_TransformPlaceholders"
    let CachedImageTransformationsAttribKey = AttributeKey "CachedImage_Transformations"
    let CachedImageDownloadStartedAttribKey = AttributeKey "CachedImage_DownloadStarted"
    let CachedImageDownloadProgressAttribKey = AttributeKey "CachedImage_DownloadProgress"
    let CachedImageFileWriteFinishedAttribKey = AttributeKey "CachedImage_FileWriteFinished"
    let CachedImageFinishAttribKey = AttributeKey "CachedImage_Finish"
    let CachedImageSuccessAttribKey = AttributeKey "CachedImage_Success"
    let CachedImageErrorAttribKey = AttributeKey "CachedImage_Error"

open ViewAttributes

[<AutoOpen>]
module FFImageLoadingExtension =
    // Fully-qualified name to avoid extending by mistake
    // another View class (like Xamarin.Forms.View)
    type Fabulous.XamarinForms.View with
        // https://github.com/luberda-molinet/FFImageLoading/wiki/Xamarin.Forms-API
        /// Describes a CachedImage in the view
        // The inline keyword is important for performance
        static member inline CachedImage
            (?source: aval<Image>, ?aspect: aval<Xamarin.Forms.Aspect>, ?isOpaque: aval<bool>, // Align first 3 parameters with Image
             ?loadingPlaceholder: aval<Image>, ?errorPlaceholder: aval<Image>,
             ?cacheType, ?cacheDuration, ?cacheKeyFactory: aval<ICacheKeyFactory>,
             ?loadingDelay, ?loadingPriority,
             ?customDataResolver: aval<FFImageLoading.Work.IDataResolver>,
             ?retryCount, ?retryDelay,
             ?downsampleWidth, ?downsampleHeight, ?downsampleToViewSize, ?downsampleUseDipUnits,
             ?fadeAnimationEnabled, ?fadeAnimationDuration, ?fadeAnimationForCachedImages,
             ?bitmapOptimizations, ?invalidateLayoutAfterLoaded,
             ?transformPlaceholders, ?transformations: alist<_>,
             ?downloadStarted, ?downloadProgress, ?fileWriteFinished, ?finish, ?success, ?error,
             // inherited attributes common to all views
             ?gestureRecognizers, ?horizontalOptions, ?margin, ?verticalOptions, ?anchorX, ?anchorY, ?backgroundColor,
             ?behaviors, ?flowDirection, ?height, ?inputTransparent, ?isEnabled, ?isTabStop, ?isVisible, ?minimumHeight,
             ?minimumWidth, ?opacity, ?resources, ?rotation, ?rotationX, ?rotationY, ?scale, ?scaleX, ?scaleY, ?styles,
             ?styleSheets, ?tabIndex, ?translationX, ?translationY, ?visual, ?width, ?style, ?styleClasses, ?shellBackButtonBehavior,
             ?shellBackgroundColor, ?shellDisabledColor, ?shellForegroundColor, ?shellFlyoutBehavior, ?shellNavBarIsVisible,
             ?shellSearchHandler, ?shellTabBarBackgroundColor, ?shellTabBarDisabledColor, ?shellTabBarForegroundColor,
             ?shellTabBarIsVisible, ?shellTabBarTitleColor, ?shellTabBarUnselectedColor, ?shellTitleColor, ?shellTitleView,
             ?shellUnselectedColor, ?automationId, ?classId, ?effects, ?menu, ?ref, ?styleId, ?tag, ?focused, ?unfocused, ?created) =
            
            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match source with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match aspect with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match isOpaque with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingPlaceholder with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match errorPlaceholder with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheDuration with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cacheKeyFactory with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingDelay with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match loadingPriority with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match customDataResolver with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match retryCount with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match retryDelay with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleWidth with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleHeight with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleToViewSize with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downsampleUseDipUnits with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationDuration with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fadeAnimationForCachedImages with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match bitmapOptimizations with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match invalidateLayoutAfterLoaded with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match transformPlaceholders with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match transformations with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downloadStarted with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match downloadProgress with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fileWriteFinished with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match finish with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match success with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match error with Some _ -> attribCount + 1 | None -> attribCount
    
            // Populate the attributes of the base element
            let attribs =
                ViewBuilders.BuildView(attribCount, ?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
                    ?verticalOptions=verticalOptions, ?anchorX=anchorX, ?anchorY=anchorY, ?backgroundColor=backgroundColor, ?behaviors=behaviors,
                    ?flowDirection=flowDirection, ?height=height, ?inputTransparent=inputTransparent, ?isEnabled=isEnabled, ?isTabStop=isTabStop,
                    ?isVisible=isVisible, ?minimumHeight=minimumHeight, ?minimumWidth=minimumWidth, ?opacity=opacity, ?resources=resources,
                    ?rotation=rotation, ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?scaleX=scaleX, ?scaleY=scaleY, ?styles=styles,
                    ?styleSheets=styleSheets, ?tabIndex=tabIndex, ?translationX=translationX, ?translationY=translationY, ?visual=visual, ?width=width,
                    ?style=style, ?styleClasses=styleClasses, ?shellBackButtonBehavior=shellBackButtonBehavior, ?shellBackgroundColor=shellBackgroundColor,
                    ?shellDisabledColor=shellDisabledColor, ?shellForegroundColor=shellForegroundColor, ?shellFlyoutBehavior=shellFlyoutBehavior,
                    ?shellNavBarIsVisible=shellNavBarIsVisible, ?shellSearchHandler=shellSearchHandler, ?shellTabBarBackgroundColor=shellTabBarBackgroundColor,
                    ?shellTabBarDisabledColor=shellTabBarDisabledColor, ?shellTabBarForegroundColor=shellTabBarForegroundColor,
                    ?shellTabBarIsVisible=shellTabBarIsVisible, ?shellTabBarTitleColor=shellTabBarTitleColor, ?shellTabBarUnselectedColor=shellTabBarUnselectedColor,
                    ?shellTitleColor=shellTitleColor, ?shellTitleView=shellTitleView, ?shellUnselectedColor=shellUnselectedColor, ?automationId=automationId,
                    ?classId=classId, ?effects=effects, ?menu=menu, ?ref=ref, ?styleId=styleId, ?tag=tag, ?focused=focused, ?unfocused=unfocused, ?created=created)
            let attribs = attribs.Retarget<CachedImage>()
                    
            // The incremental update method
            let updater1 = ViewExtensions.ValueUpdater(source, (fun (target: CachedImage) v -> target.Source <- ViewConverters.convertFabulousImageToXamarinFormsImageSource v))
            let updater2 = ViewExtensions.ValueUpdater(aspect, (fun (target: CachedImage) v -> target.Aspect <- v))
            let updater3 = ViewExtensions.ValueUpdater(isOpaque, (fun (target: CachedImage) v -> target.IsOpaque <- v))
            let updater4 = ViewExtensions.ValueUpdater(loadingPlaceholder, (fun (target: CachedImage) v -> target.LoadingPlaceholder <- ViewConverters.convertFabulousImageToXamarinFormsImageSource v))
            let updater5 = ViewExtensions.ValueUpdater(errorPlaceholder, (fun (target: CachedImage) v -> target.ErrorPlaceholder <- ViewConverters.convertFabulousImageToXamarinFormsImageSource v))
            let updater6 = ViewExtensions.ValueUpdater(cacheType, (fun (target: CachedImage) v -> target.CacheType <- Option.toNullable v))
            let updater7 = ViewExtensions.ValueUpdater(cacheDuration, (fun (target: CachedImage) v -> target.CacheDuration <- Option.toNullable v))
            let updater8 = ViewExtensions.ValueUpdater(cacheKeyFactory, (fun (target: CachedImage) v -> target.CacheKeyFactory <- v))
            let updater9 = ViewExtensions.ValueUpdater(loadingDelay, (fun (target: CachedImage) v -> target.LoadingDelay <- Option.toNullable v))
            let updater10 = ViewExtensions.ValueUpdater(loadingPriority, (fun (target: CachedImage) v -> target.LoadingPriority <- v))
            let updater11 = ViewExtensions.ValueUpdater(customDataResolver, (fun (target: CachedImage) v -> target.CustomDataResolver <- v))
            let updater12 = ViewExtensions.ValueUpdater(retryCount, (fun (target: CachedImage) v -> target.RetryCount <- v))
            let updater13 = ViewExtensions.ValueUpdater(retryDelay, (fun (target: CachedImage) v -> target.RetryDelay <- v))
            let updater14 = ViewExtensions.ValueUpdater(downsampleWidth, (fun (target: CachedImage) v -> target.DownsampleWidth <- v))
            let updater15 = ViewExtensions.ValueUpdater(downsampleHeight, (fun (target: CachedImage) v -> target.DownsampleHeight <- v))
            let updater16 = ViewExtensions.ValueUpdater(downsampleToViewSize, (fun (target: CachedImage) v -> target.DownsampleToViewSize <- v))
            let updater17 = ViewExtensions.ValueUpdater(downsampleUseDipUnits, (fun (target: CachedImage) v -> target.DownsampleUseDipUnits <- v))
            let updater18 = ViewExtensions.ValueUpdater(fadeAnimationEnabled, (fun (target: CachedImage) v -> target.FadeAnimationEnabled <- v))
            let updater19 = ViewExtensions.ValueUpdater(fadeAnimationDuration, (fun (target: CachedImage) v -> target.FadeAnimationDuration <- v))
            let updater20 = ViewExtensions.ValueUpdater(fadeAnimationForCachedImages, (fun (target: CachedImage) v -> target.FadeAnimationForCachedImages <- v))
            let updater21 = ViewExtensions.ValueUpdater(bitmapOptimizations, (fun (target: CachedImage) v -> target.BitmapOptimizations <- v))
            let updater22 = ViewExtensions.ValueUpdater(invalidateLayoutAfterLoaded, (fun (target: CachedImage) v -> target.InvalidateLayoutAfterLoaded <- v))
            let updater23 = ViewExtensions.ValueUpdater(transformPlaceholders, (fun (target: CachedImage) v -> target.TransformPlaceholders <- v))
            let updater24 = ViewExtensions.ElementCollectionUpdater(transformations, (fun (target: CachedImage) -> target.Transformations))
            let updater25 = ViewExtensions.EventUpdater(downloadStarted, (fun (target: CachedImage) -> target.DownloadStarted))
            let updater26 = ViewExtensions.EventUpdater(downloadProgress, (fun (target: CachedImage) -> target.DownloadProgress))
            let updater27 = ViewExtensions.EventUpdater(fileWriteFinished, (fun (target: CachedImage) -> target.FileWriteFinished))
            let updater28 = ViewExtensions.EventUpdater(finish, (fun (target: CachedImage) -> target.Finish))
            let updater29 = ViewExtensions.EventUpdater(success, (fun (target: CachedImage) -> target.Success))
            let updater30 = ViewExtensions.EventUpdater(error, (fun (target: CachedImage) -> target.Error))

            // Add our own attributes. They must have unique names which must match the names below.
            match source with None -> () | Some v -> attribs.Add (CachedImageSourceAttribKey, v, updater1)
            match aspect with None -> () | Some v -> attribs.Add (AspectAttribKey, v, updater2)
            match isOpaque with None -> () | Some v -> attribs.Add (IsOpaqueAttribKey, v, updater3)
            match loadingPlaceholder with None -> () | Some v -> attribs.Add (CachedImageLoadingPlaceholderAttribKey, v, updater4)
            match errorPlaceholder with None -> () | Some v -> attribs.Add (CachedImageErrorPlaceholderAttribKey, v, updater5)
            match cacheType with None -> () | Some v -> attribs.Add (CachedImageCacheTypeAttribKey, v, updater6)
            match cacheDuration with None -> () | Some v -> attribs.Add (CachedImageCacheDurationAttribKey, v, updater7)
            match cacheKeyFactory with None -> () | Some v -> attribs.Add (CachedImageCacheKeyFactoryAttribKey, v, updater8)
            match loadingDelay with None -> () | Some v -> attribs.Add (CachedImageLoadingDelayAttribKey, v, updater9)
            match loadingPriority with None -> () | Some v -> attribs.Add (CachedImageLoadingPriorityAttribKey, v, updater10)
            match customDataResolver with None -> () | Some v -> attribs.Add (CachedImageCustomDataResolverAttribKey, v, updater11)
            match retryCount with None -> () | Some v -> attribs.Add (CachedImageRetryCountAttribKey, v, updater12)
            match retryDelay with None -> () | Some v -> attribs.Add (CachedImageRetryDelayAttribKey, v, updater13)
            match downsampleWidth with None -> () | Some v -> attribs.Add (CachedImageDownsampleWidthAttribKey, v, updater14)
            match downsampleHeight with None -> () | Some v -> attribs.Add (CachedImageDownsampleHeightAttribKey, v, updater15)
            match downsampleToViewSize with None -> () | Some v -> attribs.Add (CachedImageDownsampleToViewSizeAttribKey, v, updater16)
            match downsampleUseDipUnits with None -> () | Some v -> attribs.Add (CachedImageDownsampleUseDipUnitsAttribKey, v, updater17)
            match fadeAnimationEnabled with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationEnabledAttribKey, v, updater18)
            match fadeAnimationDuration with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationDurationAttribKey, v, updater19)
            match fadeAnimationForCachedImages with None -> () | Some v -> attribs.Add (CachedImageFadeAnimationForCachedImagesAttribKey, v, updater20)
            match bitmapOptimizations with None -> () | Some v -> attribs.Add (CachedImageBitmapOptimizationsAttribKey, v, updater21)
            match invalidateLayoutAfterLoaded with None -> () | Some v -> attribs.Add (CachedImageInvalidateLayoutAfterLoadedAttribKey, v, updater22)
            match transformPlaceholders with None -> () | Some v -> attribs.Add (CachedImageTransformPlaceholdersAttribKey, v, updater23)
            match transformations with None -> () | Some v -> attribs.Add (CachedImageTransformationsAttribKey, v, updater24)
            match downloadProgress with None -> () | Some v -> attribs.Add (CachedImageDownloadProgressAttribKey, v, updater25)
            match downloadStarted with None -> () | Some v -> attribs.Add (CachedImageDownloadStartedAttribKey, v, updater26)
            match fileWriteFinished with None -> () | Some v -> attribs.Add (CachedImageFileWriteFinishedAttribKey, v, updater27)
            match finish with None -> () | Some v -> attribs.Add (CachedImageFinishAttribKey, v, updater28)
            match success with None -> () | Some v -> attribs.Add (CachedImageSuccessAttribKey, v, updater29)
            match error with None -> () | Some v -> attribs.Add (CachedImageErrorAttribKey, v, updater30)
    
            // Create a ViewElement with the instruction to create and update a CachedImage
            ViewElement.Create(CachedImage, attribs.Close())
            
#if DEBUG
    let sample =
        View.CachedImage(
            source = c (Path "path/to/image.png"),
            loadingPlaceholder = c (Path "path/to/loading-placeholder.png"),
            errorPlaceholder = c (Path "path/to/error-placeholder.png"),
            height = c 600.,
            width = c 600.
        )
#endif